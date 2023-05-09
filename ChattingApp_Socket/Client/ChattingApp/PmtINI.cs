using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;

public class PmtINI : IDisposable
{

    //
    private string strINIPath = "";

    public string IniFilePath
    {
        get
        {
            return strINIPath;
        }
        set
        {
            strINIPath = value;
        }
    }

    private Mutex p_mtx = null;
    private string HashMD5(string strOrg)
    {
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(strOrg);
        using (var md5 = System.Security.Cryptography.MD5.Create())
            bs = md5.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        return s.ToString();
    }

    private bool p_bMutexReleaseMode = false;

    private void MutexLock()
    {
        if (strINIPath == "")
            return;
        if (p_mtx == null)
        {
            string strHashName = strINIPath.ToLower();
            strHashName = HashMD5(strHashName);
            p_mtx = new Mutex(false, strHashName);
        }
        p_bMutexReleaseMode = p_mtx.WaitOne(1000);
    }

    private void MutexRelease()
    {
        if (strINIPath == "")
            return;

        if (p_mtx != null)
        {
            if (p_bMutexReleaseMode == true)
                p_mtx.ReleaseMutex();
        }
    }

    public PmtINI()
    {
        strINIPath = "";
    }

    private static List<string> p_lstCache = new List<string>();

    public PmtINI(string strIniFilePath)
    {
        strINIPath = strIniFilePath;

        string strCacheName = strIniFilePath.ToLower();
        bool bFileExists = File.Exists(strIniFilePath);
        if (bFileExists == false)
        {
            File.WriteAllText(strIniFilePath, "", Encoding.Unicode);
            p_lstCache.Remove(strCacheName);
        }
        else
        {
            if (p_lstCache.Contains(strCacheName) == true)
                return;
            p_lstCache.Add(strCacheName);
        }

        if (bFileExists == true)
        {
            FileStream stream = null;
            BinaryReader reader = null;

            try
            {
                MutexLock();

                // 유니코드 지원을 위해 UTF16-little Endian (LE) 를 확인한다.                
                var th = new System.Threading.Thread(() =>
                {
                    try
                    {
                        bool bUTF16LE = false;
                        stream = new FileStream(strIniFilePath, FileMode.Open, FileAccess.Read);
                        {
                            reader = new BinaryReader(stream);
                            {
                                //인코딩 방식	                BOM
                                //Unicode(Big - Endian)         FE FF
                                //Unicode(Little - Endian)      FF FE
                                //UTF8                          EF BB BF
                                //UTF - 32(Big - Endian)        00 00 FE FF
                                //UTF - 32(Little - Endian)     FF FE 00 00                    
                                if (reader.ReadByte() == 255 && reader.ReadByte() == 254)
                                    bUTF16LE = true;
                            }
                        }
                        try
                        {
                            if (reader != null)
                            {
                                reader.Close();
                                reader.Dispose();
                                reader = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                            Debug.WriteLine(strErrorMessage);
                        }
                        try
                        {
                            if (stream != null)
                            {
                                stream.Close();
                                stream.Dispose();
                                stream = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                            Debug.WriteLine(strErrorMessage);
                        }
                        if (bUTF16LE == false)
                        {
                            string strINIData = File.ReadAllText(strIniFilePath);
                            File.WriteAllText(strIniFilePath, strINIData, Encoding.Unicode);
                        }
                    }
                    catch (Exception ex)
                    {
                        string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                        Debug.WriteLine(strErrorMessage);
                    }
                });
                th.IsBackground = true;
                th.Start();
                if (th.Join(1000) == false)
                {
#pragma warning disable SYSLIB0006 // 형식 또는 멤버는 사용되지 않습니다.
                    th.Abort();
#pragma warning restore SYSLIB0006 // 형식 또는 멤버는 사용되지 않습니다.
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                }
                catch (Exception ex)
                {
                    string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                    Debug.WriteLine(strErrorMessage);
                }
                try
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                }
                catch (Exception ex)
                {
                    string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                    Debug.WriteLine(strErrorMessage);
                }

                MutexRelease();
            }
        }
    }

    public string ReadStr(string strSection, string strKey)
    {
        return ReadStr(strSection, strKey, "");
    }

    public string ReadStr(string strSection, string strKey, string strDefault)
    {
        if (strINIPath != "")
        {
            try
            {
                MutexLock();

                var iniMem = new PmtINIMemory(File.ReadAllText(strINIPath));
                if (iniMem.Sections.ContainsKey(strSection) == true)
                {
                    if (iniMem.Sections[strSection].ContainsKey(strKey) == true)
                    {
                        return iniMem.Sections[strSection][strKey];
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                Debug.WriteLine(strErrorMessage);
            }
            finally
            {
                MutexRelease();
            }
        }
        return strDefault;
    }

    public int Readint(string strSection, string strKey, int nDefault)
    {
        if (strINIPath == "")
            return nDefault;

        int nReturn = 0;
        if (int.TryParse(ReadStr(strSection, strKey, "0"), out nReturn) == false)
            nReturn = nDefault;
        return nReturn;
    }

    public Int64 ReadInt64(string strSection, string strKey, Int64 nDefault)
    {
        if (strINIPath == "")
            return nDefault;

        Int64 nReturn = 0;
        if (Int64.TryParse(ReadStr(strSection, strKey, "0"), out nReturn) == false)
            nReturn = nDefault;
        return nReturn;
    }

    public UInt64 ReadUInt64(string strSection, string strKey, UInt64 nDefault)
    {
        if (strINIPath == "")
            return nDefault;

        UInt64 nReturn = 0;
        if (UInt64.TryParse(ReadStr(strSection, strKey, "0"), out nReturn) == false)
            nReturn = nDefault;
        return nReturn;
    }

    public bool WriteInt(string strSection, string strKey, int nValue)
    {
        if (strINIPath == "")
            return false;

        return WriteStr(strSection, strKey, nValue.ToString());
    }

    public bool WriteStr(string strSection, string strKey, string strValue)
    {
        if (strINIPath != "")
        {
            try
            {
                MutexLock();

                var iniMem = new PmtINIMemory(File.ReadAllText(strINIPath));
                if (iniMem.Sections.ContainsKey(strSection) == false)
                    iniMem.Sections.Add(strSection, new Dictionary<string, string>());
                if (iniMem.Sections[strSection].ContainsKey(strKey) == false)
                    iniMem.Sections[strSection].Add(strKey, "");

                iniMem.Sections[strSection][strKey] = strValue;
                iniMem.Save(strINIPath);
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                Debug.WriteLine(strErrorMessage);
            }
            finally
            {
                MutexRelease();
            }
        }
        return false;
    }

    public bool DeleteKey(string strSection, string strKey)
    {
        if (strINIPath != "")
        {
            try
            {
                MutexLock();

                var iniMem = new PmtINIMemory(File.ReadAllText(strINIPath));
                if (iniMem.Sections.ContainsKey(strSection) == true)
                {
                    if (iniMem.Sections[strSection].ContainsKey(strKey) == true)
                    {
                        iniMem.Sections[strSection].Remove(strKey);
                    }
                }
                iniMem.Save(strINIPath);
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                Debug.WriteLine(strErrorMessage);
            }
            finally
            {
                MutexRelease();
            }
        }
        return false;
    }

    public bool DeleteSection(string strSection)
    {
        if (strINIPath != "")
        {
            try
            {
                MutexLock();

                var iniMem = new PmtINIMemory(File.ReadAllText(strINIPath));
                if (iniMem.Sections.ContainsKey(strSection) == true)
                {
                    iniMem.Sections.Remove(strSection);
                }
                iniMem.Save(strINIPath);
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                Debug.WriteLine(strErrorMessage);
            }
            finally
            {
                MutexRelease();
            }
        }
        return false;
    }

    public string[] GetSectionNames()  // ini 파일 안의 모든 section 이름 가져오기
    {
        if (strINIPath != "")
        {
            try
            {
                MutexLock();

                var iniMem = new PmtINIMemory(File.ReadAllText(strINIPath));
                return iniMem.Sections.Keys.ToArray();
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                Debug.WriteLine(strErrorMessage);
            }
            finally
            {
                MutexRelease();
            }
        }
        return null;
    }

    public string[] GetKeyNames(string section)   // 해당 section 안의 모든 키 값 가져오기
    {
        if (strINIPath != "")
        {
            try
            {
                MutexLock();

                var iniMem = new PmtINIMemory(File.ReadAllText(strINIPath));
                if (iniMem.Sections.ContainsKey(section) == true)
                    return iniMem.Sections[section].Keys.ToArray();
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message + ((ex.InnerException == null) ? "" : " > " + ex.InnerException.Message);
                Debug.WriteLine(strErrorMessage);
            }
            finally
            {
                MutexRelease();
            }
        }
        return null;
    }

    public void Dispose()
    {
        if (p_mtx != null)
            p_mtx.Dispose();
        p_mtx = null;
    }
}

