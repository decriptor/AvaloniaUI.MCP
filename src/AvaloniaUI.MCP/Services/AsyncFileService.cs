namespace AvaloniaUI.MCP.Services;

/// <summary>
/// Provides async file operations for improved performance and responsiveness
/// </summary>
public static class AsyncFileService
{
    /// <summary>
    /// Writes multiple files asynchronously in parallel for better performance
    /// </summary>
    public static async Task WriteAllFilesAsync(IEnumerable<(string FilePath, string Content)> files)
    {
        var writeTasks = files.Select(async file =>
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(file.FilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Write file asynchronously
                await File.WriteAllTextAsync(file.FilePath, file.Content);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to write file '{file.FilePath}': {ex.Message}", ex);
            }
        });
        
        await Task.WhenAll(writeTasks);
    }
    
    /// <summary>
    /// Writes a single file asynchronously
    /// </summary>
    public static async Task WriteFileAsync(string filePath, string content)
    {
        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            await File.WriteAllTextAsync(filePath, content);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to write file '{filePath}': {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Reads multiple files asynchronously in parallel
    /// </summary>
    public static async Task<Dictionary<string, string>> ReadAllFilesAsync(IEnumerable<string> filePaths)
    {
        var readTasks = filePaths.Select(async filePath =>
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File not found: {filePath}");
                    
                var content = await File.ReadAllTextAsync(filePath);
                return new KeyValuePair<string, string>(filePath, content);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to read file '{filePath}': {ex.Message}", ex);
            }
        });
        
        var results = await Task.WhenAll(readTasks);
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    /// <summary>
    /// Reads a single file asynchronously
    /// </summary>
    public static async Task<string> ReadFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
                
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to read file '{filePath}': {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Checks if multiple files exist asynchronously
    /// </summary>
    public static async Task<Dictionary<string, bool>> CheckFilesExistAsync(IEnumerable<string> filePaths)
    {
        var checkTasks = filePaths.Select(async filePath =>
        {
            await Task.Yield(); // Allow for async behavior even though File.Exists is sync
            return new KeyValuePair<string, bool>(filePath, File.Exists(filePath));
        });
        
        var results = await Task.WhenAll(checkTasks);
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    /// <summary>
    /// Creates a directory structure asynchronously
    /// </summary>
    public static async Task CreateDirectoryAsync(string directoryPath)
    {
        await Task.Run(() =>
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        });
    }
    
    /// <summary>
    /// Creates multiple directories asynchronously
    /// </summary>
    public static async Task CreateDirectoriesAsync(IEnumerable<string> directoryPaths)
    {
        var createTasks = directoryPaths.Select(CreateDirectoryAsync);
        await Task.WhenAll(createTasks);
    }
    
    /// <summary>
    /// Copies files asynchronously in parallel
    /// </summary>
    public static async Task CopyFilesAsync(IEnumerable<(string SourcePath, string DestinationPath)> fileCopies)
    {
        var copyTasks = fileCopies.Select(async copy =>
        {
            try
            {
                // Ensure destination directory exists
                var destinationDirectory = Path.GetDirectoryName(copy.DestinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                
                // Copy file asynchronously
                using var sourceStream = new FileStream(copy.SourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                using var destinationStream = new FileStream(copy.DestinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                await sourceStream.CopyToAsync(destinationStream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to copy file from '{copy.SourcePath}' to '{copy.DestinationPath}': {ex.Message}", ex);
            }
        });
        
        await Task.WhenAll(copyTasks);
    }
    
    /// <summary>
    /// Gets file info asynchronously for multiple files
    /// </summary>
    public static async Task<Dictionary<string, FileInfo?>> GetFileInfoAsync(IEnumerable<string> filePaths)
    {
        var infoTasks = filePaths.Select(async filePath =>
        {
            await Task.Yield(); // Allow for async behavior
            FileInfo? info = null;
            try
            {
                if (File.Exists(filePath))
                {
                    info = new FileInfo(filePath);
                }
            }
            catch
            {
                // Ignore errors, return null
            }
            return new KeyValuePair<string, FileInfo?>(filePath, info);
        });
        
        var results = await Task.WhenAll(infoTasks);
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    /// <summary>
    /// Validates file paths asynchronously
    /// </summary>
    public static async Task<Dictionary<string, ValidationResult>> ValidateFilePathsAsync(IEnumerable<string> filePaths)
    {
        var validationTasks = filePaths.Select(async filePath =>
        {
            await Task.Yield(); // Allow for async behavior
            var validation = InputValidationService.ValidateFilePath(filePath, mustExist: false);
            return new KeyValuePair<string, ValidationResult>(filePath, validation);
        });
        
        var results = await Task.WhenAll(validationTasks);
        return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    /// <summary>
    /// Processes files in batches to avoid overwhelming the system
    /// </summary>
    public static async Task ProcessFilesInBatchesAsync<T>(
        IEnumerable<T> items,
        Func<T, Task> processor,
        int batchSize = 10,
        int delayBetweenBatches = 100)
    {
        var itemsList = items.ToList();
        
        for (int i = 0; i < itemsList.Count; i += batchSize)
        {
            var batch = itemsList.Skip(i).Take(batchSize);
            var batchTasks = batch.Select(processor);
            
            await Task.WhenAll(batchTasks);
            
            // Add a small delay between batches to prevent overwhelming the system
            if (i + batchSize < itemsList.Count && delayBetweenBatches > 0)
            {
                await Task.Delay(delayBetweenBatches);
            }
        }
    }
    
    /// <summary>
    /// Safely deletes files asynchronously
    /// </summary>
    public static async Task DeleteFilesAsync(IEnumerable<string> filePaths, bool ignoreErrors = true)
    {
        var deleteTasks = filePaths.Select(async filePath =>
        {
            try
            {
                await Task.Run(() =>
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                });
            }
            catch (Exception ex)
            {
                if (!ignoreErrors)
                {
                    throw new InvalidOperationException($"Failed to delete file '{filePath}': {ex.Message}", ex);
                }
            }
        });
        
        await Task.WhenAll(deleteTasks);
    }
}