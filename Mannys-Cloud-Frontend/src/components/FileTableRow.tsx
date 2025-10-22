import type { FileDto } from "../types/ContentTypes";
import FileIcon from "./FileIcon";

type FileTableRowProps = {
  file: FileDto;
  selectable: boolean;
  isChecked: boolean;
  onChangeCheckbox: () => void;
  onSelectFile: () => void;
};
const FileTableRow = ({
  file,
  selectable,
  isChecked,
  onChangeCheckbox,
  onSelectFile,
}: FileTableRowProps) => {
  return (
    <tr className=" hover:bg-gray-800 flex px-2 gap-2">
      {selectable && (
        <td className="flex items-center">
          <input
            type="checkbox"
            checked={isChecked}
            onChange={onChangeCheckbox}
          />
        </td>
      )}
      <th
        className="flex align-items p-2 gap-2 cursor-pointer"
        onClick={onSelectFile}
      >
        <FileIcon /> {file.fileName}
      </th>
    </tr>
  );
};

export default FileTableRow;
