using Mannys_Cloud_Backend.Models;

namespace Mannys_Cloud_Backend.Util
{
    public class BuildPath
    {
        public string BuildFolderPath(Folder folder)
        {
            var userId = folder.UserId;
            var pathSegments = new List<string>();

            while (folder != null)
            {
                pathSegments.Insert(0, folder.FolderId.ToString());
                folder = folder.ParentFolder;
            }

            // Prefix with UserId (container-like namespace)    
            return $"{userId}/{string.Join("/", pathSegments)}/";
        }
    }
}
