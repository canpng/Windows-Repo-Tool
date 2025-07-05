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
using System.Collections;

namespace WindowsRepoTool
{
    public partial class Main : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private bool isInitializing = true;
        
        private int currentSortColumn = -1;
        private bool sortAscending = true;

        private readonly string dataFolder;
        private readonly string settingsPath;
        private readonly string reposPath;

        public Main()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Main_Load);
            addRepoBox.Text = "https://";
            const string sDirectory = "Debs";

            if (!Directory.Exists(sDirectory))
            {
                Directory.CreateDirectory(sDirectory);
            }

            #if DEBUG
            if (File.Exists("Languages/en.json") && !File.Exists("Languages/en.encrypted"))
            {
                EncryptLanguages.EncryptLanguageFiles();
            }
            #endif

            packageListView.ColumnClick += PackageListView_ColumnClick;

            dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataFolder);
            Directory.CreateDirectory(Path.Combine(dataFolder, "Languages"));

            settingsPath = Path.Combine(dataFolder, "Settings.json");
            reposPath = Path.Combine(dataFolder, "Repos.json");
        }

        private async void Main_Load(object? sender, EventArgs e)
        {
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

                var defaultSettings = new { Language = "tr", DarkMode = false };
                var options = new JsonSerializerOptions { WriteIndented = true };
                await File.WriteAllTextAsync(sSettings, JsonSerializer.Serialize(defaultSettings, options));
                SetSelectedLanguage("tr");
                ApplyTheme(false);
            }
            else
            {

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

                        if (settings.TryGetValue("DarkMode", out var darkModeElement) && darkModeElement.ValueKind == JsonValueKind.True)
                        {
                            ApplyTheme(true);
                        }
                        else
                        {
                            ApplyTheme(false);
                        }
                    }
                    else
                    {
                        SetSelectedLanguage("tr");
                        ApplyTheme(false);
                    }
                }
                catch
                {
                    SetSelectedLanguage("tr");
                    ApplyTheme(false);
                }
            }
            
            await LocalizationManager.LoadLanguage(GetCurrentLanguageCode());
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
            public static List<string?> architecture = new List<string?>();
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

            if (finalrepo.StartsWith("https://https://", StringComparison.OrdinalIgnoreCase))
            {
                finalrepo = finalrepo.Substring(8);
            }
            else if (finalrepo.StartsWith("http://https://", StringComparison.OrdinalIgnoreCase))
            {
                finalrepo = finalrepo.Substring(7);
            }

            if (!repoListBox.Items.Contains(finalrepo))
            {
                if (addRepoBox.Text != "https://" && addRepoBox.Text != "http://" && addRepoBox.Text != String.Empty)
                {
                    repoListBox.Items.Add(finalrepo);
                    await SaveRepos();
                    addRepoBox.Clear();
                    addRepoBox.Text = "https://";
                    MessageBox.Show(LocalizationManager.GetString("RepoAddedSuccessfully"), LocalizationManager.GetString("SuccessTitle"));
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
                packageListView.Items.Clear();
                packageDetailsBox.Clear();
                searchBox.Text = "";
                searchBox.Enabled = false;
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
                    packageListView.Items.Clear();
                    packageDetailsBox.Clear();
                    searchBox.Text = "";
                    searchBox.Enabled = false;
                    await SaveRepos();
                    MessageBox.Show(LocalizationManager.GetString("ClearedSelectedRepo"), LocalizationManager.GetString("NoticeTitle"));
                    return;
                }
            }
        }

        private async void repoListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            clearSelectedRepoBtn.Enabled = repoListBox.SelectedItem != null;

            if (repoListBox.SelectedItem == null)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            packageListView.Items.Clear();
            packageDetailsBox.Clear();
            
            searchBox.Text = "";
            searchBox.ForeColor = SystemColors.GrayText;
            searchBox.Text = LocalizationManager.GetString("SearchPackages");
            searchBox.Enabled = false;
            
            Globals.repo = repoListBox.GetItemText(repoListBox.SelectedItem);
            string? repoUrl = Globals.repo;
            const string packagesFile = "Packages";
            const string compressedFileBz2 = "Packages.bz2";
            const string compressedFileGz = "Packages.gz";

            try
            {
                Globals.name.Clear();
                Globals.version.Clear();
                Globals.link.Clear();
                Globals.details.Clear();
                Globals.package.Clear();
                Globals.architecture.Clear();

                if (Directory.Exists(packagesFile))
                {
                    Directory.Delete(packagesFile, true);
                }
                if (File.Exists(compressedFileBz2))
                {
                    File.Delete(compressedFileBz2);
                }
                if (File.Exists(compressedFileGz))
                {
                    File.Delete(compressedFileGz);
                }


                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(repoUrl + compressedFileBz2);
                        response.EnsureSuccessStatusCode();
                        using (var fileStream = new FileStream(compressedFileBz2, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
                    }

                    using (FileStream compressedStream = new FileStream(compressedFileBz2, FileMode.Open, FileAccess.Read))
                    {
                        using (var bigStream = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(compressedStream))
                        {
                            using (FileStream decompressedStream = new FileStream(packagesFile, FileMode.Create, FileAccess.Write))
                            {
                                bigStream.CopyTo(decompressedStream);
                            }
                        }
                    }
                }
                catch (HttpRequestException)
                {

                    try
                    {
                        using (var httpClient = new HttpClient())
                        {
                            var response = await httpClient.GetAsync(repoUrl + compressedFileGz);
                            response.EnsureSuccessStatusCode();
                            using (var fileStream = new FileStream(compressedFileGz, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }
                        }
                        using (FileStream compressedStream = new FileStream(compressedFileGz, FileMode.Open, FileAccess.Read))
                        {
                            using (GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                            {
                                using (FileStream decompressedStream = new FileStream(packagesFile, FileMode.Create, FileAccess.Write))
                                {
                                    decompressionStream.CopyTo(decompressedStream);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{LocalizationManager.GetString("FailedToDownloadOrDecompress")} {ex.Message}", LocalizationManager.GetString("ErrorTitle"));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{LocalizationManager.GetString("AnErrorOccurred")} {ex.Message}", LocalizationManager.GetString("ErrorTitle"));
                    return;
                }

                if (File.Exists(packagesFile))
                {
                    string[] lines = File.ReadAllLines(packagesFile);
                    string? currentPackage = null;
                    string? currentVersion = null;
                    string? currentFilename = null;
                    string? currentArchitecture = null;
                    StringBuilder currentDetails = new StringBuilder();

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Package: "))
                        {
                            currentPackage = line.Substring("Package: ".Length);
                            currentDetails.AppendLine(line);
                        }
                        else if (line.StartsWith("Version: "))
                        {
                            currentVersion = line.Substring("Version: ".Length);
                            currentDetails.AppendLine(line);
                        }
                        else if (line.StartsWith("Architecture: "))
                        {
                            currentArchitecture = line.Substring("Architecture: ".Length);
                            currentDetails.AppendLine(line);
                        }
                        else if (line.StartsWith("Filename: "))
                        {
                            currentFilename = line.Substring("Filename: ".Length);
                            currentDetails.AppendLine(line);
                        }
                        else if (string.IsNullOrWhiteSpace(line))
                        {
                            if (currentPackage != null && currentVersion != null && currentFilename != null)
                            {
                                Globals.name.Add(currentPackage);
                                Globals.version.Add(currentVersion);
                                Globals.link.Add(currentFilename);
                                Globals.details.Add(currentDetails.ToString());
                                Globals.package.Add(currentPackage);
                                Globals.architecture.Add(currentArchitecture ?? "Unknown");
                            }
                            currentPackage = null;
                            currentVersion = null;
                            currentFilename = null;
                            currentArchitecture = null;
                            currentDetails.Clear();
                        }
                        else
                        {
                            if (currentPackage != null)
                            {
                                currentDetails.AppendLine(line);
                            }
                        }
                    }

                    if (currentPackage != null && currentVersion != null && currentFilename != null)
                    {
                        Globals.name.Add(currentPackage);
                        Globals.version.Add(currentVersion);
                        Globals.link.Add(currentFilename);
                        Globals.details.Add(currentDetails.ToString());
                        Globals.package.Add(currentPackage);
                        Globals.architecture.Add(currentArchitecture ?? "Unknown");
                    }

                    for (int i = 0; i < Globals.name.Count; i++)
                    {
                        var listViewItem = new ListViewItem(new string[] { Globals.name[i], Globals.version[i], Globals.architecture[i] });
                        listViewItem.Tag = i; // Store original index
                        packageListView.Items.Add(listViewItem);
                    }
                    searchBox.Enabled = true;
                    searchBox.ForeColor = SystemColors.GrayText;
                    searchBox.Text = LocalizationManager.GetString("SearchPackages");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LocalizationManager.GetString("ErrorProcessingRepo")} {ex.Message}", LocalizationManager.GetString("ErrorTitle"));
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void packageListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            downloadSelectedPackageBtn.Enabled = packageListView.SelectedItems.Count > 0;

            if (packageListView.SelectedItems.Count > 0)
            {
                int selectedIndex = (int)packageListView.SelectedItems[0].Tag;
                
                if (selectedIndex >= 0 && selectedIndex < Globals.details.Count)
                {
                    packageDetailsBox.Text = Globals.details[selectedIndex];
                }
                else
                {
                    packageDetailsBox.Clear();
                }
            }
            else
            {
                packageDetailsBox.Clear();
            }
        }

        private void searchBox_Enter(object? sender, EventArgs e)
        {
            if (searchBox.ForeColor == SystemColors.GrayText)
            {
                searchBox.Text = "";
                searchBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void searchBox_Leave(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.ForeColor = SystemColors.GrayText;
                searchBox.Text = LocalizationManager.GetString("SearchPackages");
            }
        }

        private void searchBox_TextChanged(object? sender, EventArgs e)
        {

            if (!searchBox.Enabled || searchBox.ForeColor == SystemColors.GrayText || Globals.name.Count == 0)
            {
                return;
            }

            string searchText = searchBox.Text.ToLower();

            packageListView.Items.Clear();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If search text is empty, show all packages
                for (int i = 0; i < Globals.name.Count; i++)
                {
                    var listViewItem = new ListViewItem(new string[] { Globals.name[i], Globals.version[i], Globals.architecture[i] });
                    listViewItem.Tag = i;
                    packageListView.Items.Add(listViewItem);
                }
            }
            else
            {
                // Otherwise, filter based on search text
                for (int i = 0; i < Globals.name.Count; i++)
                {
                    if (Globals.name[i].ToLower().Contains(searchText))
                    {
                        var listViewItem = new ListViewItem(new string[] { Globals.name[i], Globals.version[i], Globals.architecture[i] });
                        listViewItem.Tag = i;
                        packageListView.Items.Add(listViewItem);
                    }
                }
            }
        }



        private void ApplyTheme(bool isDarkMode)
        {
            if (isDarkMode)
            {
                this.BackColor = ColorTranslator.FromHtml("#1e1e1e");
                this.ForeColor = ColorTranslator.FromHtml("#dfdfdf");

                menuStrip.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                menuStrip.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                
                repoContextMenuStrip.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                repoContextMenuStrip.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                addRepoBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                addRepoBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                addRepoBtn.FlatStyle = FlatStyle.Flat;
                addRepoBtn.FlatAppearance.BorderSize = 1;
                addRepoBtn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#3c3c3c");
                clearSelectedRepoBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                clearSelectedRepoBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                clearSelectedRepoBtn.FlatStyle = FlatStyle.Flat;
                clearSelectedRepoBtn.FlatAppearance.BorderSize = 1;
                clearSelectedRepoBtn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#3c3c3c");
                clearAllReposBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                clearAllReposBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                clearAllReposBtn.FlatStyle = FlatStyle.Flat;
                clearAllReposBtn.FlatAppearance.BorderSize = 1;
                clearAllReposBtn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#3c3c3c");
                downloadSelectedPackageBtn.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                downloadSelectedPackageBtn.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                downloadSelectedPackageBtn.FlatStyle = FlatStyle.Flat;
                downloadSelectedPackageBtn.FlatAppearance.BorderSize = 1;
                downloadSelectedPackageBtn.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#3c3c3c");
                repoListBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                repoListBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                packageListView.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                packageListView.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                addRepoBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                addRepoBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                packageDetailsBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                packageDetailsBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                searchBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                searchBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;

                menuStrip.BackColor = SystemColors.MenuBar;
                menuStrip.ForeColor = SystemColors.MenuText;
                
                repoContextMenuStrip.BackColor = SystemColors.Menu;
                repoContextMenuStrip.ForeColor = SystemColors.MenuText;
                addRepoBtn.BackColor = SystemColors.Control;
                addRepoBtn.ForeColor = SystemColors.ControlText;
                addRepoBtn.FlatStyle = FlatStyle.Standard;
                clearSelectedRepoBtn.BackColor = SystemColors.Control;
                clearSelectedRepoBtn.ForeColor = SystemColors.ControlText;
                clearSelectedRepoBtn.FlatStyle = FlatStyle.Standard;
                clearAllReposBtn.BackColor = SystemColors.Control;
                clearAllReposBtn.ForeColor = SystemColors.ControlText;
                clearAllReposBtn.FlatStyle = FlatStyle.Standard;
                downloadSelectedPackageBtn.BackColor = SystemColors.Control;
                downloadSelectedPackageBtn.ForeColor = SystemColors.ControlText;
                downloadSelectedPackageBtn.FlatStyle = FlatStyle.Standard;
                repoListBox.BackColor = SystemColors.Window;
                repoListBox.ForeColor = SystemColors.WindowText;
                packageListView.BackColor = SystemColors.Window;
                packageListView.ForeColor = SystemColors.WindowText;
                addRepoBox.BackColor = SystemColors.Window;
                addRepoBox.ForeColor = SystemColors.WindowText;
                packageDetailsBox.BackColor = SystemColors.Window;
                packageDetailsBox.ForeColor = SystemColors.WindowText;
                searchBox.BackColor = SystemColors.Window;
                searchBox.ForeColor = SystemColors.WindowText;
            }

            lightModeToolStripMenuItem.Checked = !isDarkMode;
            darkModeToolStripMenuItem.Checked = isDarkMode;
            if (searchBox.ForeColor == SystemColors.GrayText || string.IsNullOrWhiteSpace(searchBox.Text) || searchBox.Text == LocalizationManager.GetString("SearchPackages"))
            {
                searchBox.ForeColor = SystemColors.GrayText;
                searchBox.Text = LocalizationManager.GetString("SearchPackages");
            }
        }

        private async Task SaveSettings()
        {
            try
            {
                var settings = new 
                { 
                    Language = GetCurrentLanguageCode(),
                    DarkMode = darkModeToolStripMenuItem.Checked
                };
                var options = new JsonSerializerOptions { WriteIndented = true };
                await File.WriteAllTextAsync(settingsPath, JsonSerializer.Serialize(settings, options));
            }
            catch (Exception ex)
            {
                // Log error but don't show to user
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        private void ApplyLocalization()
        {
            this.Text = LocalizationManager.GetString("AppTitle");
            addRepoBtn.Text = LocalizationManager.GetString("AddRepo");
            clearAllReposBtn.Text = LocalizationManager.GetString("RemoveAllRepos");
            clearSelectedRepoBtn.Text = LocalizationManager.GetString("RemoveSelectedRepo");
            downloadSelectedPackageBtn.Text = LocalizationManager.GetString("DownloadSelectedPackage");
            
            settingsToolStripMenuItem.Text = LocalizationManager.GetString("MenuSettings");
            viewToolStripMenuItem.Text = LocalizationManager.GetString("MenuView");
            aboutToolStripMenuItem.Text = LocalizationManager.GetString("MenuAbout");
            languageToolStripMenuItem.Text = LocalizationManager.GetString("MenuLanguage");
            themeToolStripMenuItem.Text = LocalizationManager.GetString("MenuTheme");
            lightModeToolStripMenuItem.Text = LocalizationManager.GetString("LightMode");
            darkModeToolStripMenuItem.Text = LocalizationManager.GetString("DarkMode");
            
            editRepoToolStripMenuItem.Text = LocalizationManager.GetString("ContextMenuEdit");
            deleteRepoToolStripMenuItem.Text = LocalizationManager.GetString("ContextMenuDelete");
            
            columnHeaderName.Text = LocalizationManager.GetString("PackageNameColumn");
            columnHeaderVersion.Text = LocalizationManager.GetString("VersionColumn");
            columnHeaderArchitecture.Text = LocalizationManager.GetString("ArchitectureColumn");

            englishToolStripMenuItem.Checked = GetCurrentLanguageCode() == "en";
            turkishToolStripMenuItem.Checked = GetCurrentLanguageCode() == "tr";
            if (searchBox.ForeColor == SystemColors.GrayText || string.IsNullOrWhiteSpace(searchBox.Text) || searchBox.Text == LocalizationManager.GetString("SearchPackages"))
            {
                searchBox.ForeColor = SystemColors.GrayText;
                searchBox.Text = LocalizationManager.GetString("SearchPackages");
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

        private string GetCurrentLanguageCode()
        {
            return englishToolStripMenuItem.Checked ? "en" : "tr";
        }

        private async void downloadSelectedPackageBtn_Click(object? sender, EventArgs e)
        {
            if (packageListView.SelectedItems.Count == 0)
            {
                MessageBox.Show(LocalizationManager.GetString("PleaseSelectPackage"), LocalizationManager.GetString("NoticeTitle"));
                return;
            }

            int selectedIndex = (int)packageListView.SelectedItems[0].Tag;
            
            // Add bounds checking
            if (selectedIndex < 0 || selectedIndex >= Globals.package.Count)
            {
                MessageBox.Show("Seçilen paket bilgisi bulunamadı.", LocalizationManager.GetString("ErrorTitle"));
                return;
            }
            
            string? url = Globals.repo + Globals.link[selectedIndex];
            string? package = Globals.package[selectedIndex];
            string? version = Globals.version[selectedIndex];
            string? architecture = Globals.architecture[selectedIndex];
            string originalFileName = Path.GetFileName(url);
            string extension = Path.GetExtension(originalFileName);
            
            // Create new filename: Package+Version+Architecture+Extension
            string newFileName = $"{package}_{version}_{architecture}{extension}";
            string destinationPath = Path.Combine("Debs", newFileName);

            try
            {
                // Modern download indication: Change cursor and window title
                this.Cursor = Cursors.AppStarting;
                string originalTitle = this.Text;
                this.Text = $"İndiriliyor: {package} v{version} - {originalTitle}";
                downloadSelectedPackageBtn.Enabled = false;
                downloadSelectedPackageBtn.Text = "İndiriliyor...";

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        var totalBytes = response.Content.Headers.ContentLength;

                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                            {
                                var totalBytesRead = 0L;
                                var buffer = new byte[8192];
                                var isMoreToRead = true;

                                do
                                {
                                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                    if (bytesRead == 0)
                                    {
                                        isMoreToRead = false;
                                    }
                                    else
                                    {
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                        totalBytesRead += bytesRead;

                                        // Update window title with progress
                                        if (totalBytes.HasValue)
                                        {
                                            var progressPercentage = (int)((totalBytesRead * 100) / totalBytes.Value);
                                            this.Text = $"İndiriliyor: {package} v{version} (%{progressPercentage}) - {originalTitle}";
                                        }
                                    }
                                }
                                while (isMoreToRead);
                            }
                        }
                    }
                }

                // Restore original state
                this.Text = originalTitle;
                downloadSelectedPackageBtn.Enabled = true;
                downloadSelectedPackageBtn.Text = LocalizationManager.GetString("DownloadSelectedPackage");
                
                MessageBox.Show($"{package} v{version} ({architecture}) {LocalizationManager.GetString("DownloadedSuccessfully")}", LocalizationManager.GetString("SuccessTitle"));
            }
            catch (Exception ex)
            {
                // Restore original state on error
                this.Text = LocalizationManager.GetString("AppTitle");
                downloadSelectedPackageBtn.Enabled = true;
                downloadSelectedPackageBtn.Text = LocalizationManager.GetString("DownloadSelectedPackage");
                
                MessageBox.Show($"{LocalizationManager.GetString("DownloadFailed")} {ex.Message}", LocalizationManager.GetString("ErrorTitle"));
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void PackageListView_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            // If the same column is clicked, reverse the sort order
            if (e.Column == currentSortColumn)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                // New column clicked, default to ascending
                currentSortColumn = e.Column;
                sortAscending = true;
            }

            // Sort the ListView
            packageListView.ListViewItemSorter = new ListViewItemComparer(e.Column, sortAscending);
            packageListView.Sort();
        }

        // Menu event handlers
        private async void englishToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            await ChangeLanguage("en");
        }

        private async void turkishToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            await ChangeLanguage("tr");
        }

        private async void lightModeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyTheme(false);
            await SaveSettings();
        }

        private async void darkModeToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            ApplyTheme(true);
            await SaveSettings();
        }

        private void aboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(LocalizationManager.GetString("AboutText"), LocalizationManager.GetString("AboutTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task ChangeLanguage(string languageCode)
        {
            if (!isInitializing)
            {
                await LocalizationManager.LoadLanguage(languageCode);
                ApplyLocalization();
                await SaveSettings();
            }
        }

        private void SetSelectedLanguage(string code)
        {
            englishToolStripMenuItem.Checked = code == "en";
            turkishToolStripMenuItem.Checked = code == "tr";
        }

        private void addRepoBox_TextChanged(object? sender, EventArgs e)
        {
            string text = addRepoBox.Text.Trim();
            addRepoBtn.Enabled = !string.IsNullOrEmpty(text) && text != "https://" && text != "http://";
        }

        private async void editRepoToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (repoListBox.SelectedItem == null)
            {
                MessageBox.Show(LocalizationManager.GetString("PleaseSelectRepo"), LocalizationManager.GetString("WarningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentRepo = repoListBox.GetItemText(repoListBox.SelectedItem);
            string? newRepo = ShowInputDialog(
                LocalizationManager.GetString("EditRepoTitle"),
                LocalizationManager.GetString("EditRepoPrompt"),
                currentRepo
            );

            if (string.IsNullOrWhiteSpace(newRepo) || newRepo == currentRepo)
                return;

            if (!newRepo.StartsWith("http://") && !newRepo.StartsWith("https://"))
            {
                newRepo = "https://" + newRepo;
            }
            for (int i = 0; i < repoListBox.Items.Count; i++)
            {
                if (i != repoListBox.SelectedIndex && repoListBox.GetItemText(repoListBox.Items[i]) == newRepo)
                {
                    MessageBox.Show(LocalizationManager.GetString("RepoAlreadyAdded"), LocalizationManager.GetString("WarningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int selectedIndex = repoListBox.SelectedIndex;
            repoListBox.Items[selectedIndex] = newRepo;
            repoListBox.SelectedIndex = selectedIndex;

            await SaveRepos();

            MessageBox.Show(LocalizationManager.GetString("RepoAddedSuccessfully"), LocalizationManager.GetString("SuccessTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void deleteRepoToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (repoListBox.SelectedItem == null)
            {
                MessageBox.Show(LocalizationManager.GetString("PleaseSelectRepo"), LocalizationManager.GetString("WarningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedRepo = repoListBox.GetItemText(repoListBox.SelectedItem);
            DialogResult result = MessageBox.Show(
                $"{LocalizationManager.GetString("ConfirmClearSelectedRepo")}\n\n{selectedRepo}",
                LocalizationManager.GetString("WarningTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                repoListBox.Items.Remove(repoListBox.SelectedItem);
                
                packageListView.Items.Clear();
                packageDetailsBox.Clear();
                searchBox.Text = "";
                searchBox.ForeColor = SystemColors.GrayText;
                searchBox.Text = LocalizationManager.GetString("SearchPackages");
                searchBox.Enabled = false;

                await SaveRepos();
                MessageBox.Show(LocalizationManager.GetString("ClearedSelectedRepo"), LocalizationManager.GetString("SuccessTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string? ShowInputDialog(string title, string promptText, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 550,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 15, Top = 15, Width = 510, Text = promptText };
            TextBox textBox = new TextBox() { Left = 15, Top = 45, Width = 510, Text = defaultValue };
            Button confirmation = new Button() { Text = LocalizationManager.GetString("ButtonOK"), Left = 350, Width = 80, Top = 85, Height = 35, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = LocalizationManager.GetString("ButtonCancel"), Left = 440, Width = 80, Top = 85, Height = 35, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            if (darkModeToolStripMenuItem.Checked)
            {
                prompt.BackColor = ColorTranslator.FromHtml("#1e1e1e");
                prompt.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                textLabel.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                textBox.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                textBox.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                confirmation.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                confirmation.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                confirmation.FlatStyle = FlatStyle.Flat;
                cancel.BackColor = ColorTranslator.FromHtml("#2d2d2d");
                cancel.ForeColor = ColorTranslator.FromHtml("#dfdfdf");
                cancel.FlatStyle = FlatStyle.Flat;
            }

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }

    // Custom comparer for ListView sorting
    public class ListViewItemComparer : IComparer
    {
        private int columnIndex;
        private bool ascending;

        public ListViewItemComparer(int column, bool ascending)
        {
            this.columnIndex = column;
            this.ascending = ascending;
        }

        public int Compare(object? x, object? y)
        {
            if (x is not ListViewItem itemX || y is not ListViewItem itemY)
                return 0;

            string textX = columnIndex < itemX.SubItems.Count ? itemX.SubItems[columnIndex].Text : "";
            string textY = columnIndex < itemY.SubItems.Count ? itemY.SubItems[columnIndex].Text : "";

            int result;

            // For version column (index 1), try to do version-aware comparison
            if (columnIndex == 1)
            {
                result = CompareVersions(textX, textY);
            }
            else
            {
                // For other columns, use string comparison
                result = String.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);
            }

            return ascending ? result : -result;
        }

        private int CompareVersions(string version1, string version2)
        {
            try
            {
                // Try to parse as version numbers
                var v1Parts = version1.Split('.', '-', '+').Select(p => int.TryParse(p, out int n) ? n : 0).ToArray();
                var v2Parts = version2.Split('.', '-', '+').Select(p => int.TryParse(p, out int n) ? n : 0).ToArray();

                int maxLength = Math.Max(v1Parts.Length, v2Parts.Length);

                for (int i = 0; i < maxLength; i++)
                {
                    int part1 = i < v1Parts.Length ? v1Parts[i] : 0;
                    int part2 = i < v2Parts.Length ? v2Parts[i] : 0;

                    if (part1 != part2)
                        return part1.CompareTo(part2);
                }

                return 0;
            }
            catch
            {
                // If version parsing fails, fall back to string comparison
                return String.Compare(version1, version2, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}