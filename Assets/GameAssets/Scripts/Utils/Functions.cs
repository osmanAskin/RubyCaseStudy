using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Random = UnityEngine.Random;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

public static class Functions
{
    public static bool Android()
    {
        #if UNITY_EDITOR
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                return true;
            else
                return false;
        #endif
        
        #if UNITY_ANDROID && !UNITY_EDITOR
		    return true;
        #else
            return false;
        #endif
    }
    
    public static bool iOS()
    {
        #if UNITY_EDITOR
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                return true;
            else
                return false;
        #endif
        #if UNITY_IOS && !UNITY_EDITOR
		    return true;
        #else
            return false;
        #endif
    }

    //convert numbet to 12.123
    public static string NumberFormat( int number )
    {
        NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
        nfi.NumberDecimalDigits = 0;
        return  number.ToString( "N", nfi );
    }

    //convert number to 12K format
    public static string Number2String(float money, string defaultStringfy = "0")
    {
        if (money >= 1000000000f)
        {
            float propotion = (float)money / 1000000000f;
            var text = propotion.ToString("0.00");


            text = text.Replace(',', '.');

            return text + "b";
        }
        else if (money >= 1000000f)
        {
            float propotion = (float)money / 1000000f;
            var text = propotion.ToString("0.00");


            text = text.Replace(',', '.');

            return text + "m";
        }
        else if (money >= 1000f)
        {
            float propotion = (float)money / 1000f;
            var text = propotion.ToString("0.00");


            text = text.Replace(',', '.');

            return text + "k";
        }
        else
        {
            float remain = money % 1f;

            if(remain < 0.01f)
                return money.ToString("0");
            else
                return money.ToString(defaultStringfy);
        }
    }

    //convert second to HH:MM:SS format
    public static string Second2Hour( float sec )
    {
        int hr = 0;
        int min = 0;

        if( sec > 3600 ){
            hr = Mathf.FloorToInt(Mathf.Floor( sec / 3600f ));
            sec = Mathf.Floor( sec % 3600f );
        }
        
        if( sec > 60 ){
            min = Mathf.FloorToInt(Mathf.Floor( sec / 60f ));    
            sec = Mathf.Floor( sec % 60f );
        }
        
        string hour =  hr < 10 ? ( "0" + hr.ToString() ) : hr.ToString();
        string minute =  min < 10 ? ( "0" + min.ToString() ) : min.ToString();
        string second =  sec < 10 ? ( "0" + Mathf.FloorToInt(sec).ToString() ) : Mathf.FloorToInt(sec).ToString();
        return hour + ':' + minute + ':' + second;
    }

    //convert second to MM:SS format
    public static string Second2Min( float sec )
    {
        int min = 0;

        if( sec > 60 ){
            min = Mathf.FloorToInt(Mathf.Floor( sec / 60f ));    
            sec = Mathf.Floor( sec % 60f );
        }
        
        string minute =  min < 10 ? ( "0" + min.ToString() ) : min.ToString();
        string second =  sec < 10 ? ( "0" + Mathf.FloorToInt(sec).ToString() ) : Mathf.FloorToInt(sec).ToString();
        return minute + ':' + second;
    }
    
    //20 hr left. less then 1 min
    public static string Countdown( float sec )
    {
        string countdown;
        if( sec > (60*60*24*7))
            countdown = Mathf.CeilToInt(Mathf.Ceil( sec / (float)(60*60*24*7) )) + " wk left";
        else if( sec > (60*60*24))
            countdown = Mathf.CeilToInt(Mathf.Ceil( sec / (60f*60f*24f) )) + " d left";
        else if( sec > (60*60) )
            countdown = Mathf.CeilToInt(Mathf.Ceil( sec / 3600f )) + " hr left";
        else if( sec > 60 )
            countdown = Mathf.CeilToInt(Mathf.Ceil( sec / 60f )) + " min left";    
        else if( sec > 0 )
            countdown = "less then 1 min left";    
        else
            countdown = "Timeout!";

        return countdown;
    }

    //convert second to "2 days"
    public static string Second2String( float sec )
    {
        int time;
        string countdown;
        if( sec > (60*60*24*30)){
            time = Mathf.CeilToInt(Mathf.Ceil( sec / (float)(60*60*24*30) ));
            countdown =  time + " month" + ((time > 1 ) ? "s" : "");
        }  
        else if( sec > (60*60*24*7)){
            time = Mathf.CeilToInt(Mathf.Ceil( sec / (float)(60*60*24*7) ));
            countdown =  time + " week" + ((time > 1 ) ? "s" : "");
        }
        else if( sec > (60*60*24)){
            time = Mathf.CeilToInt(Mathf.Ceil( sec / (float)(60*60*24) ));
            countdown =  time + " day" + ((time > 1 ) ? "s" : "");
        }
        else if( sec > (60*60) ){
            time = Mathf.CeilToInt(Mathf.Ceil( sec / (float)(60*60) ));
            countdown =  time + " hour" + ((time > 1 ) ? "s" : "");
        }
        else if( sec > 60 ){
            time = Mathf.CeilToInt(Mathf.Ceil( sec / (float)(60) ));
            countdown =  time + " minute" + ((time > 1 ) ? "s" : "");
        }
        else if( sec > 0 )
            countdown = "less then 1 min";    
        else
            countdown = "";

        return countdown;
    }

    //convert second to "2d 23h 11m"
    public static string Second2Human( float sec, bool isWithSec = false )
    {
        int time;
        string countdown = "";    
        if( sec > (60*60*24)){
            time = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60*24) ));
            countdown +=  time + "d ";
            sec -= 60*60*24*time;
        }
        
        if( sec > 60*60 ){
            time = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60) ));
            countdown +=  time + "h ";
            sec -= 60*60*time;
        }

        if( sec > 60 ){
            time = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60) ));
            countdown +=  time + "m ";
            sec -= 60*time;
        }
            
        if( sec > 0 && isWithSec ){
            time = Mathf.FloorToInt(sec);
            countdown +=  time + "s ";
        } 

        return countdown.Trim();
    }


    //20 hr ago
    public static string Ago( float sec )
    {
        string countdown;

        sec = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - sec;

        if( sec > (60*60*24*30*12)){
            countdown = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60*24*30) )) + " yr";
        }
        else if( sec > (60*60*24*30)){
            countdown = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60*24*30) )) + " mo";
        }
        else if( sec > (60*60*24*7)){
            countdown = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60*24*7) )) + " wk";
        }
        else if( sec > (60*60*24)){
            countdown = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60*24) )) + " d";
        }
        else if( sec > (60*60)){
            countdown = Mathf.FloorToInt(Mathf.Floor( sec / (float)(60*60) )) + " hr";
        }
        else if( sec > 60 ){
            countdown = Mathf.FloorToInt(Mathf.Floor( sec / 60f )) + " min";    
        }
        else{
            countdown = "just before";
        }
        return countdown;
    }
    
    //20221231
    public static int GetSQLDate()
    {
        return int.Parse(DateTime.Now.ToString("yyyyMMdd"));
    }

    public static string SplitText( string text, int limit)
    {
        text = text.Length > limit * 2 ? text.Substring(0,limit * 2) : text;

        string response = "";
        foreach( string piece in text.Split(" ")){
            response += " " + (piece.Length > limit ? piece.Substring(0,limit) + " " + piece.Substring( limit + 1 ) : piece) ;
        }

        return response.Trim();
    }

    public static bool IsEmail( string value )
    {
        bool isEmail = false;
        try
        {
            isEmail = Regex.IsMatch( value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException){}
        return isEmail;
    }

    public static void ShuffleArray<T>(T[] arr) {
        for (int i = arr.Length - 1; i > 0; i--) {
            int r = Random.Range(0, i);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }

    public static long Now(long delay = 0)
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() + delay;
    }
    
    public static Coroutine StartCoroutine(this MonoBehaviour behaviour, System.Action action, float delay)
    {
        return behaviour.StartCoroutine(WaitAndDo(delay, action));
    }

    public static IEnumerator WaitAndDo(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public static string HashHMAC(string key, string message)
    {
        var hash = new HMACSHA256( StringEncode(key) );
        return HashEncode(hash.ComputeHash( StringEncode(message) ) );
    }

    public static byte[] StringEncode(string text)
    {
        var encoding = new ASCIIEncoding();
        return encoding.GetBytes(text);
    }

    public static string HashEncode(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static string Serialize(System.Object obj)
    {
        string jsonData = "";
        /*string jsonData = JsonConvert.SerializeObject(
            obj,
            Formatting.None,
            new JsonSerializerSettings { 
                NullValueHandling = NullValueHandling.Ignore, 
                DefaultValueHandling = DefaultValueHandling.Ignore  
            }
        );*/
        return jsonData;
    }
}