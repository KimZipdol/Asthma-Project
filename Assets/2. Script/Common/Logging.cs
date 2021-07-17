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
    private string LogPath = null;

    public void log(string logmsg)
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

    public string Read()
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

    //PC데이터 저장용 임시

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
        WriteCsv(input.dataList, input.savePath);
    }
}