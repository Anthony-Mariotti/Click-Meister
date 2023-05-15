using ClickMeister.Common;
using System.Globalization;
using System.IO;
using System.Windows;

namespace ClickMeister;

public class LanguageViewModel : BindableBase
{
    private const string DEFAULT_LANG = "en";

    public LanguageViewModel()
    {
        // Culture Development
        //CultureInfo.CurrentCulture = new CultureInfo("en");
        //var cultureList = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.TwoLetterISOLanguageName.StartsWith("zh"));

        // TODO: Three Letter Code? Maybe Both?
        var cultureName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        SelectedLanguage = cultureName;
        UpdateApplicationLanguage();
    }

    #region BindingSource
    private string? _selectedLanguage;

    public string? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (SetProperty(ref _selectedLanguage, value))
            {
                ApplyLanguage.RaiseCanExecuteChanged();
            }
        }
    }

    private DelegateCommand? _applyLanguage;

    public DelegateCommand ApplyLanguage => _applyLanguage ??= new(() =>
        {
            UpdateApplicationLanguage();
            ApplyLanguage.RaiseCanExecuteChanged();
        }, () =>
        {
            var resources = Application.Current.Resources;
            return resources["Language_Code"] is string lang && SelectedLanguage != lang;
        });
    #endregion

    public class LanguageTypeInfo
    {
        public string Tag { get; }

        public string Content { get; }

        public LanguageTypeInfo(string tag, string content)
        {
            Tag = tag;
            Content = content;
        }
    }

    private static ResourceDictionary? LoadLanguageResourceDictionary(string? lang = null)
    {
        try
        {
            lang = string.IsNullOrWhiteSpace(lang) ? DEFAULT_LANG : lang;
            var langUri = new Uri($@"\Resource\Language\{lang}.xaml", UriKind.Relative);
            return Application.LoadComponent(langUri) as ResourceDictionary;
        }
        catch (IOException)
        {
            return null;
        }
    }

    private void UpdateApplicationLanguage()
    {
        var langResource = LoadLanguageResourceDictionary(SelectedLanguage) ??
            LoadLanguageResourceDictionary(SelectedLanguage);

        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(langResource);
    }
}
