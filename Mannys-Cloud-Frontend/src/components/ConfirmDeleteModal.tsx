import React, { useState } from "react";
import CloseIcon from "./CloseIcon";
import toast from "react-hot-toast";
import type { Content } from "../types/ContentTypes";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

type ConfirmDeleteModalProps = {
  exitModal: () => void;
  content: Content;
};
const ConfirmDeleteModal = ({
  content,
  exitModal,
}: ConfirmDeleteModalProps) => {
  const { axios } = useAuth();
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const deleteContent = async () => {
    try {
      setLoading(true);
      if (content.fileIds.length > 0) {
        await axios.delete("/api/file/multiple", { data: content.fileIds });
      }
      if (content.folderIds.length > 0) {
        await axios.delete("/api/folder/multiple", { data: content.folderIds });
      }
      navigate(0);
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
          <>Deleting content...</>
        ) : (
          <div className="flex flex-col gap-4">
            <div
              className="absolute top-2 right-2 cursor-pointer"
              onClick={exitModal}
            >
              <CloseIcon />
            </div>
            <h2 className="text-xl">Delete Content</h2>

            <div className="flex gap-4">
              <button
                type="submit"
                className="px-4 py-2 cursor-pointer bg-green-600 hover:bg-green-700 rounded"
                onClick={deleteContent}
              >
                Confirm
              </button>
              <button
                type="submit"
                className="px-4 py-2 cursor-pointer bg-red-600 hover:bg-red-700 rounded"
                onClick={exitModal}
              >
                Cancel
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ConfirmDeleteModal;
