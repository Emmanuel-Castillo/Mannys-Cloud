import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import type { FileDto, FolderDto } from "../types/ContentTypes";
import toast from "react-hot-toast";
import LeftArrowIcon from "../components/LeftArrowIcon";
import AddContentModal from "../components/AddContentModal";
import ActionSpan from "../components/ActionSpan";
import ConfirmDeleteModal from "../components/ConfirmDeleteModal";
import ContentTable from "../components/ContentTable";
import { useContent } from "../context/ContentContext";

const HomePage = () => {
  const { authUser, axios } = useAuth();
  const { selectedContent } = useContent();
  const [currFolder, setCurrFolder] = useState<FolderDto>();
  const [folders, setFolders] = useState<FolderDto[]>([]);
  const [files, setFiles] = useState<FileDto[]>([]);
  const [selectContent, setSelectContent] = useState(false);

  // Render modal states
  const [addContent, setAddContent] = useState<"folder" | "file" | false>(
    false
  ); // AddContentModal (file/folder/false)
  const [confirmDeleteContent, setConfirmDeleteContent] = useState(false);

  // Fetching folder content
  const fetchFolderContent = async (folderId?: number) => {
    try {
      const endpoint = folderId ? `/api/folder/${folderId}` : "/api/user/root";
      const { data } = await axios.get(endpoint);
      if (data.success) {
        const rootFolder: FolderDto = data.folder;

        setCurrFolder(rootFolder);
        setFolders(rootFolder.childFolders);
        setFiles(rootFolder.files);
        setSelectContent(false);
      }
    } catch (error: any) {
      toast.error(error.message);
    }
  };
  useEffect(() => {
    fetchFolderContent();
  }, []);
  const clickFolderHandler = async (folderId: number) => {
    fetchFolderContent(folderId);
  };

  return (
    authUser &&
    currFolder && (
      <div className="p-4">
        {/* current folder name */}
        <div className="text-2xl flex gap-2">
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
        {!selectContent && (
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
              onClickButton={() => setSelectContent(true)}
            />
          </div>
        )}
        {/* selected action buttons */}
        {selectContent && (
          <div className="flex">
            <ActionSpan
              text="Delete"
              onClickButton={() => {
                setConfirmDeleteContent(true);
              }}
            />
            <ActionSpan
              text="Deselect all"
              onClickButton={() => setSelectContent(false)}
            />
          </div>
        )}

        <ContentTable
          files={files}
          folders={folders}
          selectable={selectContent}
          onSelectFolder={clickFolderHandler}
        />
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
              setSelectContent(false);
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
