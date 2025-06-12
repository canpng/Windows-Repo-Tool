using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.Json;

namespace WindowsRepoTool
{
    public partial class Main : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private bool isInitializing = true;

        // Paths
        private readonly string dataFolder;
        private readonly string settingsPath;
        private readonly string reposPath;

        public Main()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Main_Load);
            addRepoBox.Text = "https://";
            searchBox.Text = "Search Packages";
            const string sDirectory = "Debs";

            if (!Directory.Exists(sDirectory))
            {
                Directory.CreateDirectory(sDirectory);
            }

            // Enable custom drawing for language combo to support dark mode colors
            languageComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            languageComboBox.DrawItem += LanguageComboBox_DrawItem;

            // Prepare data directory structure
            dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataFolder);
            Directory.CreateDirectory(Path.Combine(dataFolder, "Languages")); // languages will be copied by build

            settingsPath = Path.Combine(dataFolder, "Settings.json");
            reposPath = Path.Combine(dataFolder, "Repos.json");
        }

        private async void Main_Load(object? sender, EventArgs e)
        {
            // Initialize language ComboBox
            languageComboBox.Items.Add(new { Name = "English", Code = "en" });
            languageComboBox.Items.Add(new { Name = "Türkçe", Code = "tr" });
            languageComboBox.DisplayMember = "Name";
            languageComboBox.ValueMember = "Code";

            string sSettings = settingsPath;
            const string sText = "Packages.txt";
            const string sPackages = "Packages";
            const string sDownloads = "Packages.bz2";
            const string sGz = "Packages.gz";

            if (Directory.Exists(sPackages))
            {
                Directory.Delete(sPackages, true);
            }
            if (File.Exists(sDownloads))
            {
                File.Delete(sDownloads);
            }
            if (File.Exists(sGz))
            {
                File.Delete(sGz);
            }
            if (File.Exists(sText))
            {
                File.Delete(sText);
            }

            if (!File.Exists(sSettings))
            {
                // Create settings file with defaults
                var defaultSettings = new { Language = "tr" };
                var options = new JsonSerializerOptions { WriteIndented = true };
                await File.WriteAllTextAsync(sSettings, JsonSerializer.Serialize(defaultSettings, options));
                darkModeBtn.Checked = false;
                SetSelectedLanguage("tr");
            }
            else
            {
                // Load existing settings
                try
                {
                    string settingsJson = await File.ReadAllTextAsync(sSettings);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(settingsJson);

                    if (settings != null)
                    {
                        if (settings.TryGetValue("Language", out var langElement) && langElement.ValueKind == JsonValueKind.String)
                        {
                            SetSelectedLanguage(langElement.GetString() ?? "tr");
                        }
                        else
                        {
                            SetSelectedLanguage("tr");
                        }
                    }
                    else // If settings are null/corrupt
                    {
                        SetSelectedLanguage("tr");
                    }
                }
                catch
                {
                    // If settings file is corrupt, load defaults
                    darkModeBtn.Checked = false;
                    SetSelectedLanguage("tr");
                }
            }
            
            await LocalizationManager.LoadLanguage(languageComboBox.SelectedValue?.ToString() ?? "tr");
            ApplyLocalization();

            await LoadRepos();
            isInitializing = false;
        }

        public static class Globals
        {
            public static List<string?> name = new List<string?>();
            public static List<string?> version = new List<string?>();
            public static List<string?> link = new List<string?>();
            public static List<string?> details = new List<string?>();
            public static List<string?> package = new List<string?>();
            public static string? repo = "";
        }

        public class ListItem
        {
            public string? Name { get; set; }
            public string? Link { get; set; }
            public string? Details { get; set; }

            public string? Package { get; set; }

            public string? Version { get; set; }

            public override string ToString()
            {
                return Name ?? string.Empty;
            }
        }

        private void Main_FormClosing(object? sender, FormClosingEventArgs e)
        {
            const string sText = "Packages.txt";
            const string sPackages = "Packages";
            const string sDownloads = "Packages.bz2";
            const string sGz = "Packages.gz";
            if (Directory.Exists(sPackages))
            {
                Directory.Delete(sPackages, true);
            }
            if (File.Exists(sDownloads))
            {
                File.Delete(sDownloads);
            }
            if (File.Exists(sGz))
            {
                File.Delete(sGz);
            }
            if (File.Exists(sText))
            {
                File.Delete(sText);
            }
        }

        private async void addRepoBtn_Click(object? sender, EventArgs e)
        {
            string repo = addRepoBox.Text.ToLower();
            string finalrepo;

            if (repo.EndsWith("/"))
            {
                finalrepo = repo;
            }
            else
            {
                finalrepo = repo + "/";
            }

            if (!repoListBox.Items.Contains(finalrepo))
            {
                if (addRepoBox.Text != "https://" && addRepoBox.Text != "http://" && addRepoBox.Text != String.Empty)
                {
                    repoListBox.Items.Add(finalrepo);
                    await SaveRepos();
                    addRepoBox.Clear();
                    addRepoBox.Text = "https://";
                    return;
                }
                else
                {
                    MessageBox.Show(LocalizationManager.GetString("TypeInARepo"), LocalizationManager.GetString("NoticeTitle"));
                    return;
                }
            }
            else
            {
                addRepoBox.Clear();
                addRepoBox.Text = "https://";
                MessageBox.Show(LocalizationManager.GetString("RepoAlreadyAdded"), LocalizationManager.GetString("NoticeTitle"));
                return;
            };
        }

        private async void clearAllReposBtn_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(LocalizationManager.GetString("ConfirmClearAllRepos"), LocalizationManager.GetString("NoticeTitle"), MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                repoListBox.Items.Clear();
                await SaveRepos();
                MessageBox.Show(LocalizationManager.GetString("ClearedAllRepos"), LocalizationManager.GetString("NoticeTitle"));
                return;
            }
        }

        private async void clearSelectedRepoBtn_Click(object? sender, EventArgs e)
        {
            if (repoListBox.SelectedItem == null)
            {
                MessageBox.Show(LocalizationManager.GetString("PleaseSelectRepo"), LocalizationManager.GetString("NoticeTitle"));
                return;
            }
            else
            {
                var result = MessageBox.Show(LocalizationManager.GetString("ConfirmClearSelectedRepo"), LocalizationManager.GetString("NoticeTitle"), MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    repoListBox.Items.Remove(repoListBox.SelectedItem);
                    await SaveRepos();
                    MessageBox.Show(LocalizationManager.GetString("ClearedSelectedRepo"), LocalizationManager.GetString("NoticeTitle"));
                    return;
                }
            }
        }

        private async void openSelectedRepoBtn_Click(object? sender, EventArgs e)
        {
            var selectedRepo = repoListBox.SelectedItem;
            if (selectedRepo == null)
            {
                MessageBox.Show(LocalizationManager.GetString("PleaseSelectRepo"), LocalizationManager.GetString("NoticeTitle"));
                return;
            }
            else
            {
                packagesListBox.Items.Clear();
                if (searchBox.Text != LocalizationManager.GetString("SearchPackages"))
                {
                    searchBox.Text = LocalizationManager.GetString("SearchPackages");
                }
                detailsBox.Clear();
                try
                {
                    const string sRepos = "https://sarahh12099.github.io/files/badrepos.txt";
                    string badReposContent = await client.GetStringAsync(sRepos);
                    using (StringReader reader = new StringReader(badReposContent))
                    {
                        string? check;
                        while ((check = reader.ReadLine()) != null)
                        {
                            if (check == selectedRepo.ToString())
                            {
                                var result = MessageBox.Show(LocalizationManager.GetString("DangerousRepoWarning"), LocalizationManager.GetString("WarningTitle"), MessageBoxButtons.YesNo);
                                if (result == DialogResult.No)
                                {
                                    return;
                                }
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not fetch bad repos list: " + ex.Message);
                }

                packagesListBox.Items.Add(LocalizationManager.GetString("OpeningRepo"));
                this.Enabled = false;

                Globals.name.Clear();
                Globals.version.Clear();
                Globals.link.Clear();
                Globals.details.Clear();
                Globals.package.Clear();
                packagesListBox.Items.Clear();

                Globals.repo = selectedRepo.ToString()!;
                const string sPath = "Packages.bz2";
                const string sGz = "Packages.gz";
                const string sFolder = "Packages";
                const string sPackages = "Packages/Packages";
                const string sText = "Packages.txt";
                if (File.Exists(sPath))
                {
                    File.Delete(sPath);
                }
                if (File.Exists(sGz))
                {
                    File.Delete(sGz);
                }
                if (File.Exists(sText))
                {
                    File.Delete(sText);
                }
                if (Directory.Exists(sFolder))
                {
                    Directory.Delete(sFolder, true);
                }

                string[] packageFileUrls =
                {
                    Globals.repo + "Packages.bz2",
                    Globals.repo + "dists/stable/main/binary-iphoneos-arm/Packages.bz2",
                    Globals.repo + "Packages.gz"
                };

                string? downloadedFilePath = null;

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-Machine", "iPhone8,1");
                client.DefaultRequestHeaders.Add("X-Unique-ID", "8843d7f92416211de9ebb963ff4ce28125932878");
                client.DefaultRequestHeaders.Add("X-Firmware", "13.1");
                client.DefaultRequestHeaders.Add("User-Agent", "Telesphoreo APT-HTTP/1.0.592");

                foreach (var url in packageFileUrls)
                {
                    try
                    {
                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var extension = Path.GetExtension(url);
                            downloadedFilePath = "Packages" + extension;
                            using (var fs = new FileStream(downloadedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                            break;
                        }
                    }
                    catch (HttpRequestException) { /* Continue to next URL */ }
                }

                if (downloadedFilePath == null)
                {
                    MessageBox.Show(LocalizationManager.GetString("ConnectionError"), LocalizationManager.GetString("ErrorTitle"));
                    this.Enabled = true;
                    packagesListBox.Items.Clear();
                    return;
                }
                
                // Try to find 7za.exe in common locations
                string[] possiblePaths = {
                    "7za.exe",
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "7-Zip", "7z.exe")
                };

                string? zPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        zPath = path;
                        break;
                    }
                }

                if (zPath == null)
                {
                    MessageBox.Show(LocalizationManager.GetString("SevenZipNotFound"), LocalizationManager.GetString("ErrorTitle"));
                    this.Enabled = true;
                    packagesListBox.Items.Clear();
                    return;
                }

                try
                {
                    await Task.Run(() => {
                        ProcessStartInfo pro = new ProcessStartInfo();
                        pro.WindowStyle = ProcessWindowStyle.Hidden;
                        pro.CreateNoWindow = true;
                        pro.UseShellExecute = false;
                        pro.FileName = zPath;
                        pro.Arguments = string.Format("x \"{0}\" -y -o\"{1}\"", downloadedFilePath, sFolder);
                        Process? x = Process.Start(pro);
                        x?.WaitForExit();
                    });
                }
                catch (Exception Ex)
                {
                    string titlefinal = "Notice";
                    string messagefinal = Ex.ToString();
                    MessageBox.Show(messagefinal, titlefinal);
                    this.Enabled = true;
                    packagesListBox.Items.Clear();
                    return;
                }

                if (!File.Exists(sPackages))
                {
                    MessageBox.Show(LocalizationManager.GetString("ExtractionError"), LocalizationManager.GetString("ErrorTitle"));
                    this.Enabled = true;
                    packagesListBox.Items.Clear();
                    return;
                }

                string packagesContent = await File.ReadAllTextAsync(sPackages);
                if (File.Exists(sText))
                {
                    File.Delete(sText);
                }

                var packageEntries = packagesContent.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

                // Use temporary lists to build package data
                var listItems = new List<ListItem>();

                foreach (var entry in packageEntries)
                {
                    string? name = null, version = null, link = null, package = null;
                    var properties = entry.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var prop in properties)
                    {
                        if (prop.StartsWith("Name: "))
                            name = prop.Substring(6).Trim();
                        else if (prop.StartsWith("Version: "))
                            version = prop.Substring(9).Trim();
                        else if (prop.StartsWith("Filename: "))
                            link = prop.Substring(10).Trim();
                        else if (prop.StartsWith("Package: "))
                            package = prop.Substring(9).Trim();
                    }

                    // Fallback to package id if name is missing
                    if (name == null && package != null)
                    {
                        name = package;
                    }

                    if (name != null && version != null && link != null && package != null)
                    {
                        string formattedEntry = entry.Replace("\r\n", "\n").Replace("\n", "\r\n");
                        listItems.Add(new ListItem { Name = $"{name} v{version}", Link = link, Details = formattedEntry, Package = package, Version = version });
                    }
                }

                // Now, clear global state and UI, and repopulate
                Globals.name.Clear();
                Globals.version.Clear();
                Globals.link.Clear();
                Globals.details.Clear();
                Globals.package.Clear();
                packagesListBox.Items.Clear();

                foreach (var item in listItems)
                {
                    // Add to UI
                    packagesListBox.Items.Add(item);

                    // Add to global state
                    Globals.name.Add(item.Name);
                    Globals.version.Add(item.Version);
                    Globals.link.Add(item.Link);
                    Globals.details.Add(item.Details);
                    Globals.package.Add(item.Package);
                }

                packagesListBox.Sorted = true;
                this.Enabled = true;
            }
        }

        private async void downloadSelectedPackageBtn_Click(object? sender, EventArgs e)
        {
            if (packagesListBox.SelectedItem is not ListItem selectedItem)
            {
                MessageBox.Show(LocalizationManager.GetString("PleaseSelectPackage"), LocalizationManager.GetString("NoticeTitle"));
                return;
            }

            if (selectedItem.Package is null || selectedItem.Version is null || selectedItem.Link is null || Globals.repo is null)
            {
                MessageBox.Show(LocalizationManager.GetString("PackageInfoIncomplete"), LocalizationManager.GetString("ErrorTitle"));
                return;
            }

            string selectedPackageItem = selectedItem.Package + "_" + selectedItem.Version;
            string repoURL = Globals.repo;
            string packageURL = selectedItem.Link;
            string downloadURL = repoURL + packageURL;
            
            if (!Directory.Exists("Debs"))
            {
                Directory.CreateDirectory("Debs");
            }

            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-Machine", "iPhone8,1");
                client.DefaultRequestHeaders.Add("X-Unique-ID", "8843d7f92416211de9ebb963ff4ce28125932878");
                client.DefaultRequestHeaders.Add("X-Firmware", "13.1");
                client.DefaultRequestHeaders.Add("User-Agent", "Telesphoreo APT-HTTP/1.0.592");
                
                byte[] fileBytes = await client.GetByteArrayAsync(downloadURL);
                await File.WriteAllBytesAsync($"Debs/{selectedPackageItem}.deb", fileBytes);

                MessageBox.Show(LocalizationManager.GetString("DownloadComplete"), LocalizationManager.GetString("NoticeTitle"));
            }
            catch (Exception)
            {
                MessageBox.Show(LocalizationManager.GetString("DownloadFailed"), LocalizationManager.GetString("NoticeTitle"));
            }
        }

        private void packagesListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            detailsBox.Clear();
            detailsBox.ScrollBars = ScrollBars.Both;
            detailsBox.WordWrap = false;
            if (packagesListBox.SelectedItem is ListItem selectedItem)
            {
                detailsBox.Text = selectedItem.Details ?? string.Empty;
            }
        }

        private void searchBox_Enter(object? sender, EventArgs e)
        {
            if (searchBox.Text == LocalizationManager.GetString("SearchPackages"))
            {
                searchBox.Clear();
            }
        }

        private void searchBox_Leave(object? sender, EventArgs e)
        {
            if (searchBox.Text == string.Empty)
            {
                searchBox.Text = LocalizationManager.GetString("SearchPackages");
            }
        }

        private void searchButton_Click(object? sender, EventArgs e)
        {
            packagesListBox.Items.Clear();
            // Repopulate based on Globals, which are not cleared
            for (int i = 0; i < Globals.name.Count; i++)
            {
                string? currentName = Globals.name[i];
                if (currentName != null && currentName.StartsWith(searchBox.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    packagesListBox.Items.Add(new ListItem { Name = Globals.name[i], Link = Globals.link[i], Details = Globals.details[i], Package = Globals.package[i], Version = Globals.version[i] });
                }
            }
            packagesListBox.Sorted = true;
        }

        private async void darkModeBtn_CheckedChanged(object? sender, EventArgs e)
        {
            if (isInitializing) return;

            ApplyTheme(darkModeBtn.Checked);
            await SaveSettings();
        }

        private void ApplyTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                this.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                repoListBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                repoListBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                packagesListBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                packagesListBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                addRepoBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                addRepoBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                detailsBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                detailsBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                searchBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                searchBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                addRepoBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                addRepoBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                addRepoBtn.FlatStyle = FlatStyle.Flat;
                addRepoBtn.FlatAppearance.BorderSize = 1;
                clearSelectedRepoBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                clearSelectedRepoBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                clearSelectedRepoBtn.FlatStyle = FlatStyle.Flat;
                clearSelectedRepoBtn.FlatAppearance.BorderSize = 1;
                clearAllReposBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                clearAllReposBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                clearAllReposBtn.FlatStyle = FlatStyle.Flat;
                clearAllReposBtn.FlatAppearance.BorderSize = 1;
                openSelectedRepoBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                openSelectedRepoBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                openSelectedRepoBtn.FlatStyle = FlatStyle.Flat;
                openSelectedRepoBtn.FlatAppearance.BorderSize = 1;
                downloadSelectedPackageBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                downloadSelectedPackageBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                downloadSelectedPackageBtn.FlatStyle = FlatStyle.Flat;
                downloadSelectedPackageBtn.FlatAppearance.BorderSize = 1;
                searchButton.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                searchButton.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                searchButton.FlatStyle = FlatStyle.Flat;
                searchButton.FlatAppearance.BorderSize = 1;
                darkModeBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                languageLabel.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                languageComboBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                languageComboBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                languageComboBox.FlatStyle = FlatStyle.Flat;
                languageComboBox.Invalidate();
            }
            else
            {
                this.BackColor = SystemColors.Control;
                repoListBox.BackColor = SystemColors.Window;
                repoListBox.ForeColor = SystemColors.WindowText;
                packagesListBox.BackColor = SystemColors.Window;
                packagesListBox.ForeColor = SystemColors.WindowText;
                addRepoBox.BackColor = SystemColors.Window;
                addRepoBox.ForeColor = SystemColors.WindowText;
                detailsBox.BackColor = SystemColors.Window;
                detailsBox.ForeColor = SystemColors.WindowText;
                searchBox.BackColor = SystemColors.Window;
                searchBox.ForeColor = SystemColors.WindowText;
                
                foreach (var button in this.Controls.OfType<Button>())
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.FlatStyle = FlatStyle.Standard;
                }

                darkModeBtn.ForeColor = SystemColors.ControlText;
                languageLabel.ForeColor = SystemColors.ControlText;
                languageComboBox.BackColor = SystemColors.Window;
                languageComboBox.ForeColor = SystemColors.WindowText;
                languageComboBox.FlatStyle = FlatStyle.Standard;
                languageComboBox.Invalidate();
            }
        }

        private async Task SaveSettings()
        {
            string sSettings = settingsPath;
            var settings = new
            {
                Language = languageComboBox.SelectedValue?.ToString() ?? "tr"
            };
            var options = new JsonSerializerOptions { WriteIndented = true };
            await File.WriteAllTextAsync(sSettings, JsonSerializer.Serialize(settings, options));
        }

        private void ApplyLocalization()
        {
            this.Text = LocalizationManager.GetString("FormTitle");
            addRepoBtn.Text = LocalizationManager.GetString("AddRepo");
            clearAllReposBtn.Text = LocalizationManager.GetString("RemoveAllRepos");
            clearSelectedRepoBtn.Text = LocalizationManager.GetString("RemoveSelectedRepo");
            openSelectedRepoBtn.Text = LocalizationManager.GetString("OpenSelectedRepo");
            downloadSelectedPackageBtn.Text = LocalizationManager.GetString("DownloadSelectedPackage");
            searchButton.Text = LocalizationManager.GetString("SearchButton");
            darkModeBtn.Text = LocalizationManager.GetString("DarkMode");
            languageLabel.Text = LocalizationManager.GetString("Language");

            // Only update the search box text if it's a placeholder or empty
            if (searchBox.Tag as string == "placeholder" || string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = LocalizationManager.GetString("SearchPackages");
                searchBox.Tag = "placeholder";
            }
        }

        private async void languageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (isInitializing) return;
            
            var selectedItem = languageComboBox.SelectedItem as dynamic;
            if (selectedItem == null) return;

            string selectedLang = selectedItem.Code;
            await LocalizationManager.LoadLanguage(selectedLang);
            ApplyLocalization();
            await SaveSettings();
        }

        private void LanguageComboBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            string text;
            if (e.Index >= 0)
            {
                text = languageComboBox.GetItemText(languageComboBox.Items[e.Index]) ?? string.Empty;
            }
            else
            {
                // Edit portion (selected item)
                text = languageComboBox.GetItemText(languageComboBox.SelectedItem) ?? string.Empty;
            }

            var font = e.Font ?? languageComboBox.Font;

            if (e.State.HasFlag(DrawItemState.Selected) && e.Index >= 0)
            {
                e.Graphics.DrawString(text, font, SystemBrushes.HighlightText, e.Bounds);
            }
            else
            {
                using var brush = new SolidBrush(languageComboBox.ForeColor);
                e.Graphics.DrawString(text, font, brush, e.Bounds);
            }
        }

        private async Task SaveRepos()
        {
            var repoItems = repoListBox.Items.Cast<string>().ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            await File.WriteAllTextAsync(reposPath, JsonSerializer.Serialize(repoItems, options));
        }

        private async Task LoadRepos()
        {
            if (File.Exists(reposPath))
            {
                try
                {
                    string json = await File.ReadAllTextAsync(reposPath);
                    var list = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                    repoListBox.Items.AddRange(list.ToArray());
                }
                catch
                {
                    // if corrupt, ignore
                }
            }
            else
            {
                Directory.CreateDirectory(dataFolder);
                await File.WriteAllTextAsync(reposPath, "[]");
            }
        }

        private void SetSelectedLanguage(string code)
        {
            for (int i = 0; i < languageComboBox.Items.Count; i++)
            {
                dynamic item = languageComboBox.Items[i];
                if (item.Code == code)
                {
                    languageComboBox.SelectedIndex = i;
                    return;
                }
            }
            // fallback first
            if (languageComboBox.Items.Count > 0)
                languageComboBox.SelectedIndex = 0;
        }
    }
}