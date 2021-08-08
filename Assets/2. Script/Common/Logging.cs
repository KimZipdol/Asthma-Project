using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class Logging : MonoBehaviour
{

    private string directory = null;
    private string path = "";
    //DirectoryInfo directoryInfo = null;
    FileStream fs = null;
    StreamWriter writer = null;

    int count = 1;

    private void Start()
    {
        //path = pathForDocumentsFile("pressurelog.txt");
        //directoryInfo = new DirectoryInfo(path);
        directory = Application.persistentDataPath + "/Log";
        path = directory + "/log.txt";
        
    }

    //안드로이드 로그
    //
    //
    public void logPressure(string pressure)
    {
        //Debug.Log("logging");
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }


        //디렉토리에 파일이 없으면 만들고, 있으면 열어서 끝으로 이동.  
        if (!File.Exists(path))
        {
            fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        }
        else
        {
            fs = new FileStream(path, FileMode.Append, FileAccess.Write);
        }
        writer = new StreamWriter(fs);
        // 파일이 65000줄 보다 길어 엑셀처리 힘들경우 기존 데이터 닫고 다시 만들기
        if (fs.Length > 65000)
        {
            fs.Flush();
            fs.Close();
            fs = new FileStream(directory + "log" + count +".txt", FileMode.CreateNew, FileAccess.Write);
            path = directory + "log" + count + ".txt";
            count++;
        }

        
        string logfrm = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.fff") + ", " + pressure;  //작성 내용. 임시로 현재날짜쓰게돼있음.
        writer.WriteLine(logfrm);
        writer.Close();
        fs.Close();
    }

    public void logClearTime(string clearTime)
    {
        //디렉토리에 파일이 없으면 만들고, 있으면 열어서 끝으로 이동.  
        if (!File.Exists(path))
        {
            fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        }
        else
        {
            fs = new FileStream(path, FileMode.Append, FileAccess.Write);
        }
        // 파일이 65000줄 보다 길어 엑셀처리 힘들경우 기존 데이터 닫고 다시 만들기
        if (fs.Length > 65000)
        {
            fs.Flush();
            fs.Close();
            fs = new FileStream(directory + "log" + count + ".txt", FileMode.CreateNew, FileAccess.Write);
            path = directory + "log" + count + ".txt";
            count++;
        }

        StreamWriter writer = new StreamWriter(fs);
        string logfrm = DateTime.Now.ToString("yyyyMMdd hh:mm:ss.fff") + ", ClearTime, " + clearTime;  //작성 내용. 임시로 현재날짜쓰게돼있음.
        writer.WriteLine(logfrm);
        writer.Close();
        fs.Close();
    }

    

    public void writeStringToFile(string str, string filename)
    {
#if !WEB_BUILD
        string path = pathForDocumentsFile(filename);
        FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

        StreamWriter sw = new StreamWriter(file);
        sw.WriteLine(str);

        sw.Close();
        file.Close();
#endif
    }


    

    //파일 이름을 받아 저장 경로를 리턴하는 함수
    public string pathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }

        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
    }


    /*
    //PC데이터 저장용 임시
    //https://coderzero.tistory.com/entry/%EC%9C%A0%EB%8B%88%ED%8B%B0-%EC%8A%A4%ED%81%AC%EB%A6%BD%ED%8A%B8-%EC%86%8C%EC%8A%A4-csv-%ED%8C%8C%EC%9D%BC-%EC%9D%BD%EA%B3%A0-%EC%93%B0%EA%B8%B0
    //위 사이트 참고함
    private bool m_IsWriting;

    //CSV 읽기 메서드
    public string[,] ReadCsv(string filePath)
    {
        string value = "";
        StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
        value = reader.ReadToEnd();
        reader.Close();

        string[] lines = value.Split("\n"[0]);

        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        string[,] outputGrid = new string[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    public string[] SplitCsvLine(string line)
    {
        return (from Match m in System.Text.RegularExpressions.Regex.Matches(line,@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            RegexOptions.ExplicitCapture)select m.Groups[1].Value).ToArray();
    }

    //CSV 쓰기 메서드
    public void WriteCsv(List<string[]> rowData, string filePath)
    {
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder stringBuilder = new StringBuilder();

        for (int index = 0; index < length; index++)
            stringBuilder.AppendLine(string.Join(delimiter, output[index]));

        Stream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        StreamWriter outStream = new StreamWriter(fileStream, Encoding.UTF8);
        outStream.WriteLine(stringBuilder);
        outStream.Close();

        m_IsWriting = false;

        Debug.Log("CSV 저장 완료");
    }

    public struct inputSensorData
    {
        public List<string[]> dataList;
        public string savePath;
    }

    public void WriteCsvFile(inputSensorData input)
    {
        Debug.Log("전달완료");
        WriteCsv(input.dataList, input.savePath);
    }

    */
}