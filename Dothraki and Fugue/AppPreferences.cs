using Android.Content;
using Android.Preferences;

public class AppPreferences
{
    private ISharedPreferences mSharedPrefs;
    private ISharedPreferencesEditor mPrefsEditor;
    private Context mContext;

    private static string PREFERENCE_ACCESS_KEY = "PREFERENCE_ACCESS_KEY";

    public AppPreferences(Context context)
    {
        this.mContext = context;
        mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
        mPrefsEditor = mSharedPrefs.Edit();
    }

    public void saveAccessKey(string key)
    {
        mPrefsEditor.PutString(PREFERENCE_ACCESS_KEY, key);
        mPrefsEditor.Commit();
    }

    public string getAccessKey()
    {
        return mSharedPrefs.GetString(PREFERENCE_ACCESS_KEY, "");
    }


}