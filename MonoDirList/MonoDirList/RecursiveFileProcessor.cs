using System;
using System.IO;
using System.Collections;
using System.Threading;


// For Directory.GetFiles and Directory.GetDirectories
// For File.Exists, Directory.Exists
// Recursive file processor class. Modified to be IBM i specific but should work on Windows machines as well. 
// Sample class was found on Microsoft site. We are able to keep dir lists from endless looping on 
// symbolic and other links which are not actually files or directories by using the FileAttributes.Reparsepoint 
// setting check on a file. This is important because the IFS file system has many symbolic links that could cause 
// endless loop issues by technically processing the same subdirs over and over because of symbolic links.
//https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?redirectedfrom=MSDN&view=netframework-4.7.2#System_IO_Directory_GetFiles_System_String_
public class RecursiveFileProcessor
{

    private long dircount = 0;
    private long filecount = 0;
    private long symboliclinkcount = 0;
    private long invalidcount = 0;
    private long totalbytecount = 0;
    private int sleepms = 0;
    private int curblockcount = 0;
    private int sleepblockcount = 1000; // Sleep after every so many objects processed
    private TimeZoneInfo tzone = null;

    /// <summary>
    /// Set time zone info value
    /// </summary>
    /// <param name="timezone"></param>
    public void SetTimeZone(TimeZoneInfo timezone)
    {
        tzone = timezone;
    }

    /// <summary>
    /// Set thread sleep throttle. If 0, no thread sleeping
    /// </summary>
    /// <param name="milliseconds"></param>
    public void SetSleepThrottle(int milliseconds)
    {
        sleepms = milliseconds;
    }

    /// <summary>
    /// Set thread sleep block count. Sleep every xx dir entries
    /// </summary>
    /// <param name="blockcount">Sleep every xx records. Default=1000 records.</param>
    public void SetSleepBlockCount(int blockcount=1000)
    {
        sleepblockcount = blockcount;
    }

    /// <summary>
    /// Get file count
    /// </summary>
    /// <returns></returns>
    public long GetFileCount()
    {
        return filecount;
    }
    /// <summary>
    /// Get directory count
    /// </summary>
    /// <returns></returns>
    public long GetDirCount()
    {
        return dircount;
    }
    /// <summary>
    /// Get symbolic link count from directory list
    /// </summary>
    /// <returns></returns>
    public long GetSymbolicLinkCount()
    {
        return symboliclinkcount;
    }
    /// <summary>
    /// Get invalid link count from directory list
    /// </summary>
    /// <returns></returns>
    public long GetInvalidCount()
    {
        return invalidcount;
    }
    /// <summary>
    /// Get total byte count form directory list
    /// </summary>
    /// <returns></returns>
    public long GetTotalByteCount()
    {
        return totalbytecount;
    }

    /// <summary>
    /// Process selected list of top level directories and recurse subdirectories as well
    /// </summary>
    /// <param name="paths">List of paths</param>
    /// <param name="outputfile">Output file</param>
    public void IterateDirectories(string[] paths, string outputfile)
    {

        File.WriteAllText(outputfile, "FILEDIRTYP,FILEDIRNAM,DIRNAME,FILENAME,FILEEXT,FILEDIRSIZ,CRTTIME,LSTWRTTIME,LSTACCTIME,CRTTIMEISO,LSTWRTISO,LSTACCISO,\r\n");

        //RecursiveFileProcessor recurse1 = new RecursiveFileProcessor();

        foreach (string path in paths)
        {
            if (File.Exists(path))
            {

                FileAttributes fa = File.GetAttributes(path);

                //See if directory is a symbolic link. If so, skip it
                //This should prevent endless looping if we try to walk a 
                //symbolic link path because it creates a never-ending loop cycle
                if ((fa & FileAttributes.ReparsePoint) != 0)
                {
                    //throw new Exception("File " + parmdirectory + " appears to be a symbolic link.");
                    File.AppendAllText(outputfile, "SYMLNKFILE," + path + "," + path + ", , ,0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000" + "\r\n");
                    symboliclinkcount++;
                }
                else
                {
                    // This path is a file
                    ProcessFile(path, outputfile);
                }
            }
            else if (Directory.Exists(path))
            {

                //DirectoryInfo di = new DirectoryInfo(path);
                //FileAttributes fa = di.Attributes;

                FileAttributes fa = File.GetAttributes(path);

                //See if directory is a symbolic link
                if ((fa & FileAttributes.ReparsePoint) != 0)
                {
                    //throw new Exception("File " + parmdirectory + " appears to be a symbolic link.");
                    File.AppendAllText(outputfile, "SYMLNKDIR," + path + "," + path + ", , ,0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000" + "\r\n");
                    symboliclinkcount++;
                }
                else
                {
                    // This path is a directory
                    ProcessDirectory(path, outputfile);
                }

            }
            else
            {
                File.AppendAllText(outputfile, "INVALID," + path + "," + path + ", , ,0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000" + "\r\n");
                //Console.WriteLine("{0} is not a valid file or directory.", path);
                invalidcount++;
            }
        }

    }

    // Process all files in the directory passed in, recurse on any directories 
    // that are found, and process the files they contain.
    public void ProcessDirectory(string targetDirectory,string outputfile)
    {

        // Throttle our thread so we don't kill the CPU
        if (sleepms > 0)
        {
            if (curblockcount > sleepblockcount)
            {
                Thread.Sleep(sleepms);
                curblockcount = 0; // Reset current block count
            }
        }

        FileAttributes fa = File.GetAttributes(targetDirectory);

        //See if directory is a symbolic link. If so, skip it
        //This should prevent endless looping if we try to walk a 
        //symbolic link path because it creates a never-ending loop cycle
        if ((fa & FileAttributes.ReparsePoint) != 0)
        {
            //throw new Exception("File " + parmdirectory + " appears to be a symbolic link.");
            File.AppendAllText(outputfile, "SYMLNKDIR," + targetDirectory + "," + targetDirectory + ", , ,0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000" + "\r\n");
            symboliclinkcount++;
            curblockcount++; 

        } else if (Directory.Exists(targetDirectory) == false) { // Checking symbolic links on IBMi gets file not found
            //throw new Exception("File " + parmdirectory + " appears to be a symbolic link.");
            File.AppendAllText(outputfile, "SYMLNKDIRNOTFOUND," + targetDirectory + "," + targetDirectory + ", , ,0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000,1900-01-01-00.00.00.000000,1900-01-01-00.00.00.000000" + "\r\n");
            symboliclinkcount++;
            curblockcount++;
        }
        else // Looks to be a real directory and not a symbolic link
        {
            DirectoryInfo di = new DirectoryInfo(targetDirectory);
            //Convert dir dates based on current timezone info from the custom timezone. This is needed because mono date/time on dirs doesn't convert to current UTC timezone offset automatically.
            File.AppendAllText(outputfile, "DIR," + targetDirectory + "," + targetDirectory + ", , ," + "0" + "," + TimeZoneInfo.ConvertTime(di.CreationTimeUtc, tzone).ToString() + "," + TimeZoneInfo.ConvertTime(di.LastWriteTimeUtc, tzone).ToString() + "," + TimeZoneInfo.ConvertTime(di.LastAccessTimeUtc, tzone).ToString() + "," + ConvertTimeToIsoString(TimeZoneInfo.ConvertTime(di.CreationTimeUtc, tzone)) + "," + ConvertTimeToIsoString(TimeZoneInfo.ConvertTime(di.LastWriteTimeUtc, tzone)) + "," + ConvertTimeToIsoString(TimeZoneInfo.ConvertTime(di.LastAccessTimeUtc, tzone)) + "\r\n");
            dircount++;
            curblockcount++;

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName, outputfile);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory, outputfile);
        }

    }

    // Insert logic for processing found files here.
    public void ProcessFile(string path,string outputfile)
    {

        // Throttle our thread so we don't kill the CPU
        if (sleepms > 0)
        {
            if (curblockcount > sleepblockcount)
            {
                Thread.Sleep(sleepms);
                curblockcount = 0; // Reset current block count
            }
        }

        FileAttributes fa = File.GetAttributes(path);

        //See if directory is a symbolic link. If so, skip it
        //This should prevent endless looping if we try to walk a 
        //symbolic link path because it creates a never-ending loop cycle
        if ((fa & FileAttributes.ReparsePoint) != 0)
        {
            //throw new Exception("File " + parmdirectory + " appears to be a symbolic link.");
            File.AppendAllText(outputfile, "SYMLNKFILE," + path + "," + Path.GetDirectoryName(path)  + "," + Path.GetFileName(path) + "," + Path.GetExtension(path).Replace(".","").PadRight(1).ToLower() + ",0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000" + "\r\n");
            symboliclinkcount++;
            curblockcount++;

        }
        else if (File.Exists(path) == false) { // Checking symbolic links on IBMi gets file not found
            //throw new Exception("File " + parmdirectory + " appears to be a symbolic link.");
            File.AppendAllText(outputfile, "SYMLNKFILENOTFOUND," + path + "," + Path.GetDirectoryName(path) + "," + Path.GetFileName(path) + "," + Path.GetExtension(path).Replace(".", "").PadRight(1).ToLower() + ",0,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,,1/1/1980 12:00:00 AM,1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000, 1900-01-01-00.00.00.000000" + "\r\n");
            symboliclinkcount++;
            curblockcount++;

        }
        else
        { // Looks to be a real file and not a symbolic link
            //Console.WriteLine("Processed file '{0}'.", path);
            FileInfo fi = new FileInfo(path);
            //Convert file dates based on current timezone info from the custom timezone. This is needed because mono date/time on dirs doesn't convert to current UTC timezone offset automatically.
            File.AppendAllText(outputfile, "FILE," + path + "," + Path.GetDirectoryName(path) + "," + Path.GetFileName(path) + "," + Path.GetExtension(path).Replace(".", "").PadRight(1).ToLower() +  "," + fi.Length + "," + TimeZoneInfo.ConvertTime(fi.CreationTimeUtc, tzone).ToString() + "," + TimeZoneInfo.ConvertTime(fi.LastWriteTimeUtc, tzone).ToString() + "," + TimeZoneInfo.ConvertTime(fi.LastAccessTimeUtc, tzone).ToString() + "," + ConvertTimeToIsoString(TimeZoneInfo.ConvertTime(fi.CreationTimeUtc, tzone)) + "," + ConvertTimeToIsoString( TimeZoneInfo.ConvertTime(fi.LastWriteTimeUtc, tzone)) + "," + ConvertTimeToIsoString(TimeZoneInfo.ConvertTime(fi.LastAccessTimeUtc, tzone)) + "\r\n");
            filecount++;
            curblockcount++;

            // Add to total file size count
            totalbytecount = totalbytecount + fi.Length;
        }
    }

    /// <summary>
    /// Convert date/time to ISO date format for IBM i storage in DB2 ISO field type 
    /// HH:mm:ss is stored in 24hr military time for ISO
    /// </summary>
    /// <param name="timevalue">DateTime value to format</param>
    /// <returns>Converted date or 1/1/1900 on error</returns>
    string ConvertTimeToIsoString(DateTime timevalue)
    {
        try
        {
            return String.Format("{0:yyyy-MM-dd-HH.mm.ss.000000}", timevalue);
        } catch (Exception ex) { 
            return "1900-01-01-00.00.00.000000";
        }
    }

}