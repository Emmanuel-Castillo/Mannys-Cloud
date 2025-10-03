import React, { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { type FileDto, type FolderDto } from "../types/ContentTypes";
import toast from "react-hot-toast";
import FolderIcon from "../components/FolderIcon";
import FileIcon from "../components/FileIcon";
import LeftArrowIcon from "../components/LeftArrowIcon";
import ViewFileModal from "../components/ViewFileModal";

const HomePage = () => {
  const { authUser, logout, axios } = useAuth();
  const [currFolder, setCurrFolder] = useState<FolderDto>();
  const [folders, setFolders] = useState<FolderDto[]>([]);
  const [files, setFiles] = useState<FileDto[]>([]);

  const [viewingFile, setViewingFile] = useState<FileDto>();

  const fetchRootFolderContent = async (folderId?: number) => {
    try {
      const endpoint = folderId ? `/api/folder/${folderId}` : "/api/user/root";
      const { data } = await axios.get(endpoint);
      console.log(data);
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
        <div className="flex justify-between align-items-center mb-3 ">
          <h1>Welcome, {authUser.fullName}</h1>
          <button
            onClick={logout}
            className="p-3 bg-gradient-to-r from-red-400 to-red-600 rounded-md cursor-pointer"
          >
            Logout
          </button>
        </div>
        <div className="mt-10">
          <p className="text-2xl mb-3 flex gap-2">
            {currFolder.parentFolderId && (
              <div
                onClick={() => clickFolderHandler(currFolder.parentFolderId)}
                className="cursor-pointer"
              >
                <LeftArrowIcon />
              </div>
            )}
            Current folder: {currFolder.folderName}
          </p>
          <div className="flex">
            <span className="py-2 px-4 hover:bg-gray-600 cursor-pointer">
              Add folder+
            </span>
            <span className="py-2 px-4 hover:bg-gray-600 cursor-pointer">
              Add file+
            </span>
            <span className="py-2 px-4 hover:bg-gray-600 cursor-pointer">
              Delete
            </span>
            {/* <span className="py-2 px-4 hover:bg-gray-600 cursor-pointer">Rename</span> */}
          </div>
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
                  className="cursor-pointer hover:bg-gray-800"
                  onClick={() => clickFolderHandler(f.folderId)}
                >
                  <th className="flex align-items p-2 gap-2 ">
                    <FolderIcon /> {f.folderName}
                  </th>
                </tr>
              ))}
              {files.map((f) => (
                <tr
                  key={f.fileId}
                  className="cursor-pointer hover:bg-gray-800"
                  onClick={() => setViewingFile(f)}
                >
                  <th className="flex align-items p-2 gap-2 ">
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

        {viewingFile && <ViewFileModal file={viewingFile} exitModal={() => setViewingFile(undefined)}/>  }
      </div>
    )
  );
};

export default HomePage;
