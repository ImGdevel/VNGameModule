using System;
using System.IO;
using System.IO.Compression;

public class FolderCompression
{
    public static void CompressFolder(string sourceFolderPath, string targetFilePath) {
        try {
            // 폴더를 압축합니다.
            ZipFile.CreateFromDirectory(sourceFolderPath, targetFilePath);
        }
        catch (Exception e) {
            Console.WriteLine("폴더 압축 중 오류 발생: " + e.Message);
        }
    }

    public static void ExtractZipFile(string sourceZipFilePath, string targetFolder) {
        try {
            // 압축 파일을 지정된 폴더로 해체합니다.
            ZipFile.ExtractToDirectory(sourceZipFilePath, targetFolder);
        }
        catch (Exception e) {
            Console.WriteLine("압축 해제 중 오류 발생: " + e.Message);
        }
    }

    public static void ChangeExtension(string sourceFilePath, string newExtension) {
        try {
            // 파일의 확장자를 변경합니다.
            string targetFilePath = Path.ChangeExtension(sourceFilePath, newExtension);
            File.Move(sourceFilePath, targetFilePath);
        }
        catch (Exception e) {
            Console.WriteLine("확장자 변경 중 오류 발생: " + e.Message);
        }
    }
}
