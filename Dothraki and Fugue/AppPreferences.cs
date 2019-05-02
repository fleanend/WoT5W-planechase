using Android.Content;
using Android.Preferences;

public class AppPreferences
{
    private ISharedPreferences mSharedPrefs;
    private ISharedPreferencesEditor mPrefsEditor;
    private Context mContext;

    private static string DECK_LIST = "DECK_LIST";

    public AppPreferences(Context context)
    {
        this.mContext = context;
        mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
        mPrefsEditor = mSharedPrefs.Edit();
    }

    public void saveDeckList(string deck_list)
    {
        mPrefsEditor.PutString(DECK_LIST, deck_list);
        mPrefsEditor.Commit();
    }

    public string loadDeckList()
    {
        return mSharedPrefs.GetString(DECK_LIST, "");
    }



}