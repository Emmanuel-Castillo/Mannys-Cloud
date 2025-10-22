import { useEffect, useState } from "react";
import type { FileDto, FolderDto } from "../types/ContentTypes";
import ViewFileModal from "./ViewFileModal";
import { useContent } from "../context/ContentContext";
import FolderTableRowProps from "./FolderTableRow";
import FileTableRow from "./FileTableRow";

type ContentTableProps = {
  folders: FolderDto[];
  files: FileDto[];
  selectable: boolean;
  onSelectFolder?: (folderId: number) => void;
};
const ContentTable = ({
  folders,
  files,
  selectable,
  onSelectFolder,
}: ContentTableProps) => {
  const [viewingFile, setViewingFile] = useState<FileDto>(); // ViewFileModal
  const { selectedContent, setSelectedContent } = useContent();

  useEffect(() => {
    setSelectedContent({
      fileIds: [],
      folderIds: [],
    });
  }, [selectable]);

  return (
    <>
      <table className="w-full rounded border border-opacity-75 border-separate">
        <thead className="flex p-2 bg-gray-600">
          <tr>
            <th>Name</th>
          </tr>
        </thead>
        <tbody>
          {folders.map((f) => (
            <FolderTableRowProps
              key={f.folderId}
              folder={f}
              selectable={selectable}
              isChecked={selectedContent.folderIds.includes(f.folderId)}
              onChangeCheckbox={() =>
                setSelectedContent((prev) => ({
                  ...prev,
                  folderIds: prev.folderIds.includes(f.folderId)
                    ? prev.folderIds.filter((p) => p !== f.folderId)
                    : [...prev.folderIds, f.folderId],
                }))
              }
              onSelectFolder={() =>
                onSelectFolder && onSelectFolder(f.folderId)
              }
            />
          ))}

          {files.map((f) => (
            <FileTableRow
              key={f.fileId}
              file={f}
              isChecked={selectedContent.fileIds.includes(f.fileId)}
              selectable={selectable}
              onChangeCheckbox={() =>
                setSelectedContent((prev) => ({
                  ...prev,
                  fileIds: prev.fileIds.includes(f.fileId)
                    ? prev.fileIds.filter((p) => p !== f.fileId)
                    : [...prev.fileIds, f.fileId],
                }))
              }
              onSelectFile={() => setViewingFile(f)}
            />
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
      {viewingFile && (
        <ViewFileModal
          file={viewingFile}
          exitModal={() => setViewingFile(undefined)}
        />
      )}
    </>
  );
};

export default ContentTable;
