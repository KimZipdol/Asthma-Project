using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

public class Logging
{
    static string directory = Application.persistentDataPath + "/Log";
    static string LogPath = Application.persistentDataPath + "/Log/log.txt";

    public static void log(string logmsg)
    {
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        FileStream fs = null;
        fs = new FileStream(LogPath, FileMode.Append);
        if (fs.Length > 2048000)
        {
            fs.Close();
            // 기존 데이터 비우고 다시 열기
            fs = new FileStream(LogPath, FileMode.Create, FileAccess.Write);
        }

        StreamWriter writer = new StreamWriter(fs);
        string logfrm = DateTime.Now.ToString("yyyyMMdd hh:mm:ss") + " " + logmsg;  //작성 내용. 임시로 현재날짜쓰게돼있음.
        writer.WriteLine(logfrm);
        writer.Close();
        fs.Close();
    }

    public static string Read()
    {
        StreamReader file = File.OpenText(LogPath);
        bool end = file.EndOfStream;
        string temp = "";
        while (!end)
        {
            temp = file.ReadLine();
            end = file.EndOfStream;
        }
        file.Close();        //파일을 닫아요  

        return temp;
    }
}