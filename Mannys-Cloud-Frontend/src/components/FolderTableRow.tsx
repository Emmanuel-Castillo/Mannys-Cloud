import FolderIcon from "./FolderIcon";
import type { FolderDto } from "../types/ContentTypes";

type FolderTableRowProps = {
  folder: FolderDto;
  selectable: boolean;
  isChecked: boolean;
  onChangeCheckbox: () => void;
  onSelectFolder: () => void;
};
const FolderTableRowProps = ({
  folder,
  selectable,
  isChecked,
  onChangeCheckbox,
  onSelectFolder,
}: FolderTableRowProps) => {
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
        onClick={onSelectFolder}
      >
        <FolderIcon /> {folder.folderName}
      </th>
    </tr>
  );
};

export default FolderTableRowProps;
