using Supabase.Gotrue.Exceptions;
using Supabase.Gotrue;
using System;
using TMPro;
using UnityEngine;
using Postgrest.Attributes;
using Postgrest.Models;

public class SupabaseSignIn : MonoBehaviour 
{
    public const string SUPABASE_URL = "https://vehgfmsvoubmgbsliodd.supabase.co";

    public const string SUPABASE_PUBLIC_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZlaGdmbXN2b3VibWdic2xpb2RkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDg1Mjg0NjQsImV4cCI6MjA2NDEwNDQ2NH0.2qsEZvH02JlZVEc3MONlAen5CYdsFldIGcpiK-PS6Sw";

    public string email;
    public string password;
    public string username = "nameee";

    public TextMeshProUGUI textBox;

    private static Supabase.Client _supabase;
    public async void Start()
    {
        if (_supabase == null)
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
                AutoRefreshToken = true
            };
            _supabase = new Supabase.Client(SUPABASE_URL, SUPABASE_PUBLIC_KEY, options);
            await _supabase.InitializeAsync();

            string accessToken = PlayerPrefs.GetString("access_token", null);
            string refreshToken = PlayerPrefs.GetString("refresh_token", null);
            Debug.Log(accessToken + " + " + refreshToken);


            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    await _supabase.Auth.SetSession(accessToken, refreshToken);
                    Debug.Log($"Restored session: {_supabase.Auth.CurrentUser?.Email}");
                    textBox.text = $"Restored session: {_supabase.Auth.CurrentUser?.Email}";
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to restore session: {ex.Message}");

                    textBox.text = $"Failed to restore session: {ex.Message}";

                }
            }
            else
            {
                LoginUser();
            }



        }

    }

    public async void RegisterUser()
    {
        try
        {

            Session session = (await _supabase.Auth.SignUp(email, password))!;
            Debug.Log($"Success! Signed Up as {session.User?.Email}");

        }
        catch (GotrueException goTrueException)
        {
            Debug.Log(goTrueException.Message);
            Debug.LogException(goTrueException);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public async void LoginUser()
    {
        try
        {
            Session session = (await _supabase.Auth.SignIn(email, password))!;
            Debug.Log($"Success! Signed in as {session.User?.Email}");

            textBox.text = $"Success! Signed in as {session.User?.Email}";

            PlayerPrefs.SetString("access_token", session.AccessToken);
            PlayerPrefs.SetString("refresh_token", session.RefreshToken);
            PlayerPrefs.Save();

            Debug.Log(session.AccessToken);
        }
        catch (GotrueException goTrueException)
        {
            Debug.Log(goTrueException.Message);
            Debug.LogException(goTrueException);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e);
        }
    }

    private async void TestInsert()
    {
        var model = new TestTable
        {
            TextData = "The Shire",
        };
        await _supabase.From<TestTable>().Insert(model);

        var result = await _supabase.From<TestTable>().Get();
        var cities = result.Models;
        Debug.Log(cities.Count);


    }


}


[Table("TestTable")]
class TestTable : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    [Column("textData")]
    public string TextData { get; set; }

}