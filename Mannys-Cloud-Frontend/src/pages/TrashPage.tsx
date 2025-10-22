import { useEffect, useState } from "react";
import ActionSpan from "../components/ActionSpan";
import ContentTable from "../components/ContentTable";
import type { FileDto, FolderDto } from "../types/ContentTypes";
import { useAuth } from "../context/AuthContext";
import toast from "react-hot-toast";

const TrashPage = () => {
  const { axios } = useAuth();
  const [selectContent, setSelectContent] = useState(false);
  const [files, setFiles] = useState<FileDto[]>([]);
  const [folders, setFolders] = useState<FolderDto[]>([]);

  useEffect(() => {
    const fetchTrash = async () => {
      try {
        const { data } = await axios.get("/api/user/trash");
        if (data.success) {
          setFiles(data.trashFiles);
          setFolders(data.trashFolders);
        } else {
          toast.error(data.error);
        }
      } catch (error: any) {
        toast.error(error.message);
      }
    };

    fetchTrash()
  }, []);

  return (
    <div className="p-3">
      <div className="text-2xl flex gap-2">Trash</div>
      {/* action buttons */}
      {!selectContent && (
        <div className="flex">
          <ActionSpan
            text="Select all"
            onClickButton={() => setSelectContent(true)}
          />
        </div>
      )}
      {/* selected action buttons */}
      {selectContent && (
        <div className="flex">
          <ActionSpan text="Restore" onClickButton={() => {}} />
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
      />
    </div>
  );
};

export default TrashPage;
