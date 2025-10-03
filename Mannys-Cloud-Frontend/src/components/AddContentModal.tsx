import React, { useState } from "react";
import CloseIcon from "./CloseIcon";
import { useAuth } from "../context/AuthContext";
import toast from "react-hot-toast";
import { useNavigate } from "react-router-dom";

type AddContentModalProps = {
  userId: number;
  parentFolderId: number;
  contentType: "folder" | "file";
  exitModal: () => void;
};
const AddContentModal = ({
  userId,
  parentFolderId,
  contentType,
  exitModal,
}: AddContentModalProps) => {
  const { axios } = useAuth();
  const navigate = useNavigate();
  const [folderName, setFolderName] = useState("");
  const [newFile, setNewFile] = useState<File>();

  const [loading, setLoading] = useState(false);

  const formHandler = async () => {
    try {
      setLoading(true);
      let request: {} | FormData;
      if (contentType === "folder") {
        if (folderName.length === 0) throw new Error("Name is empty!");
        request = { userId, folderName, parentFolderId };
      } else {
        if (newFile === undefined) throw new Error("File not uploaded!");
        const formData = new FormData();
        formData.append("file", newFile);
        formData.append("userId", userId.toString());
        formData.append("folderId", parentFolderId.toString());
        request = formData;
      }
      const { data } = await axios.post(`/api/${contentType}`, request);
      if (data.success) {
        navigate(0);
        return;
      }
    } catch (error: any) {
      toast.error(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/50" onClick={exitModal}></div>
      <div className="relative z-10 bg-gray-900 rounded-lg p-6">
        {loading ? (
          <>Uploading content...</>
        ) : (
          <form
            className="flex flex-col gap-4"
            onSubmit={(e) => {
              e.preventDefault();
              formHandler();
            }}
          >
            <div
              className="absolute top-2 right-2 cursor-pointer"
              onClick={exitModal}
            >
              <CloseIcon />
            </div>
            <h2 className="text-xl">Add {contentType}</h2>

            {contentType === "folder" && (
              <div>
                <p>Name:</p>
                <input
                  type="text"
                  value={folderName}
                  className="rounded p-2 bg-gray-700"
                  placeholder={`Enter name`}
                  onChange={(e) => setFolderName(e.target.value)}
                />
              </div>
            )}

            {contentType === "file" && (
              <div>
                <p>Upload file: </p>
                <input
                  type="file"
                  className="bg-gray-600 rounded p-1"
                  onChange={(e) => {
                    if (e.target.files && e.target.files?.length > 0) {
                      setNewFile(e.target.files[0]);
                    }
                  }}
                />
              </div>
            )}

            <button
              type="submit"
              className="px-4 py-2 cursor-pointer bg-green-600 hover:bg-green-700 rounded"
            >
              Submit
            </button>
          </form>
        )}
      </div>
    </div>
  );
};

export default AddContentModal;
