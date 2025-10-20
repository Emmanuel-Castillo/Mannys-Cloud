import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import type { Content, FileDto, FolderDto } from "../types/ContentTypes";
import toast from "react-hot-toast";
import FolderIcon from "../components/FolderIcon";
import FileIcon from "../components/FileIcon";
import LeftArrowIcon from "../components/LeftArrowIcon";
import ViewFileModal from "../components/ViewFileModal";
import AddContentModal from "../components/AddContentModal";
import ActionSpan from "../components/ActionSpan";
import ConfirmDeleteModal from "../components/ConfirmDeleteModal";

const HomePage = () => {
  const { authUser, logout, axios } = useAuth();
  const [currFolder, setCurrFolder] = useState<FolderDto>();
  const [folders, setFolders] = useState<FolderDto[]>([]);
  const [files, setFiles] = useState<FileDto[]>([]);

  const [selectedContent, setSelectedContent] = useState<Content>({
    fileIds: [],
    folderIds: [],
  });

  useEffect(() => {
    console.log(selectedContent);
  }, [selectedContent]);
  const contentSelected =
    selectedContent.fileIds.length > 0 || selectedContent.folderIds.length > 0;

  // Render modal states
  const [viewingFile, setViewingFile] = useState<FileDto>(); // ViewFileModal
  const [addContent, setAddContent] = useState<"folder" | "file" | false>(
    false
  ); // AddContentModal (file/folder/false)
  const [confirmDeleteContent, setConfirmDeleteContent] = useState(false);

  const fetchRootFolderContent = async (folderId?: number) => {
    try {
      const endpoint = folderId ? `/api/folder/${folderId}` : "/api/user/root";
      const { data } = await axios.get(endpoint);
      if (data.success) {
        const rootFolder: FolderDto = data.folder;

        setCurrFolder(rootFolder);
        setFolders(rootFolder.childFolders);
        setFiles(rootFolder.files);
      }
    } catch (error: any) {
      toast.error(error.message);
    }
  };

  useEffect(() => {
    fetchRootFolderContent();
  }, []);

  const clickFolderHandler = async (folderId: number) => {
    fetchRootFolderContent(folderId);
  };

  return (
    authUser &&
    currFolder && (
      <div className="h-screen w-full p-12">
        {/* navbar */}
        <div className="flex justify-between align-items-center mb-3 ">
          <h1>Welcome, {authUser.fullName}</h1>
          <button
            onClick={logout}
            className="p-3 bg-gradient-to-r from-red-400 to-red-600 rounded-md cursor-pointer"
          >
            Logout
          </button>
        </div>

        {/* body */}
        <div className="mt-10">
          {/* current folder name */}
          <div className="text-2xl mb-3 flex gap-2">
            {currFolder.parentFolderId && (
              <div
                onClick={() => clickFolderHandler(currFolder.parentFolderId)}
                className="cursor-pointer"
              >
                <LeftArrowIcon />
              </div>
            )}
            Current folder: {currFolder.folderName}
          </div>

          {/* action buttons */}
          {!contentSelected && (
            <div className="flex">
              <ActionSpan
                text="Add folder+"
                onClickButton={() => setAddContent("folder")}
              />
              <ActionSpan
                text="Add file+"
                onClickButton={() => setAddContent("file")}
              />
              <ActionSpan
                text="Select all"
                onClickButton={() => {
                  setSelectedContent({
                    fileIds: currFolder.files.map((f) => f.fileId),
                    folderIds: currFolder.childFolders.map((f) => f.folderId),
                  });
                }}
              />
            </div>
          )}

          {/* selected action buttons */}
          {contentSelected && (
            <div className="flex">
              <ActionSpan
                text="Delete"
                onClickButton={() => {
                  setConfirmDeleteContent(true);
                }}
              />
              <ActionSpan
                text="Deselect all"
                onClickButton={() => {
                  setSelectedContent({ fileIds: [], folderIds: [] });
                }}
              />
            </div>
          )}

          <table className="w-full rounded border border-opacity-75 border-separate">
            <thead className="flex p-2 bg-gray-600">
              <tr>
                <th>Name</th>
              </tr>
            </thead>
            <tbody>
              {folders.map((f) => (
                <tr
                  key={f.folderId}
                  className=" hover:bg-gray-800 flex px-2 gap-2"
                >
                  {contentSelected && (
                    <td className="flex items-center">
                      <input
                        type="checkbox"
                        checked={selectedContent.folderIds.includes(f.folderId)}
                        onChange={() =>
                          setSelectedContent((prev) => ({
                            ...prev,
                            folderIds: prev.folderIds.includes(f.folderId)
                              ? prev.folderIds.filter((p) => p !== f.folderId)
                              : [...prev.folderIds, f.folderId],
                          }))
                        }
                      />
                    </td>
                  )}
                  <th
                    className="flex align-items p-2 gap-2 cursor-pointer"
                    onClick={() => clickFolderHandler(f.folderId)}
                  >
                    <FolderIcon /> {f.folderName}
                  </th>
                </tr>
              ))}
              {files.map((f) => (
                <tr
                  key={f.fileId}
                  className=" hover:bg-gray-800 flex px-2 gap-2"
                >
                  {contentSelected && (
                    <td className="flex items-center">
                      <input
                        type="checkbox"
                        checked={selectedContent.fileIds.includes(f.fileId)}
                        onChange={() =>
                          setSelectedContent((prev) => ({
                            ...prev,
                            fileIds: prev.fileIds.includes(f.fileId)
                              ? prev.fileIds.filter((p) => p !== f.fileId)
                              : [...prev.fileIds, f.fileId],
                          }))
                        }
                      />
                    </td>
                  )}
                  <th
                    className="flex align-items p-2 gap-2 cursor-pointer"
                    onClick={() => setViewingFile(f)}
                  >
                    <FileIcon /> {f.fileName}
                  </th>
                </tr>
              ))}
              {folders.length == 0 && files.length == 0 && (
                <tr>
                  <th className="flex align-items p-2 gap-2 ">
                    No items in this folder
                  </th>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        {viewingFile && (
          <ViewFileModal
            file={viewingFile}
            exitModal={() => setViewingFile(undefined)}
          />
        )}
        {addContent && (
          <AddContentModal
            userId={authUser.userId}
            parentFolderId={currFolder.folderId}
            contentType={addContent}
            exitModal={() => setAddContent(false)}
          />
        )}
        {confirmDeleteContent && (
          <ConfirmDeleteModal
            exitModal={() => {
              setSelectedContent({ fileIds: [], folderIds: [] });
              setConfirmDeleteContent(false);
            }}
            content={selectedContent}
          />
        )}
      </div>
    )
  );
};

export default HomePage;
