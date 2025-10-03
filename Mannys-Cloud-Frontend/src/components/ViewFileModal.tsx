import toast from "react-hot-toast";
import { useAuth } from "../context/AuthContext";
import type { FileDto } from "../types/ContentTypes";
import CloseIcon from "./CloseIcon";

type ViewFileModalProps = {
  file: FileDto;
  //   fileId: number;
  //     userId: number;
  //     fileName: string;
  //     blobPath: string;
  //     contentType: string;
  //     sizeBytes: number;
  //     folderId: number | null;
  //     createdAt: string;
  //     isDeleted: boolean
  exitModal: () => void;
};
import { useState } from "react";

const ViewFileModal = ({ file, exitModal }: ViewFileModalProps) => {
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const { axios } = useAuth();

  const handlePreview = async () => {
    try {
      const response = await axios.get(`/api/file/${file.fileId}/download`, {
        responseType: "blob",
      });

      const blobUrl = URL.createObjectURL(response.data);
      setPreviewUrl(blobUrl);
    } catch (error: any) {
      toast.error(error.message);
    }
  };

  const handleDownload = async () => {
    try {
      const response = await axios.get(`/api/file/${file.fileId}/download`, {
        responseType: "blob",
      });

      const blob = new Blob([response.data], { type: file.contentType });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");

      // filename from backend header
      const disposition = response.headers["content-disposition"];
      let filename = file.fileName;
      if (disposition) {
        const match = disposition.match(/filename="?([^"]+)"?/);
        if (match?.[1]) filename = match[1];
      }

      link.href = url;
      link.setAttribute("download", filename);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (error: any) {
      toast.error(error.message);
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/50" onClick={exitModal}></div>
      <div className="relative z-10 h-[90%] w-[60%] bg-gray-900 rounded-lg shadow-lg p-6 text-white overflow-auto">
        <div
          className="absolute top-2 right-2 cursor-pointer"
          onClick={exitModal}
        >
          <CloseIcon />
        </div>

        <h2 className="text-xl mb-4">{file.fileName}</h2>

        <p>
          <b>Type:</b> {file.contentType}
        </p>
        <p>
          <b>Size:</b> {file.sizeBytes} bytes
        </p>
        <p>
          <b>Created:</b> {new Date(file.createdAt).toLocaleString()}
        </p>

        <div className="mt-4 flex gap-4">
          <button
            onClick={handlePreview}
            className="px-4 py-2 cursor-pointer bg-blue-600 hover:bg-blue-700 rounded"
          >
            Preview
          </button>
          <button
            onClick={handleDownload}
            className="px-4 py-2 cursor-pointer bg-green-600 hover:bg-green-700 rounded"
          >
            Download
          </button>
        </div>

        {previewUrl && (
          <div className="mt-6 border border-gray-700 rounded p-2 bg-black flex items-center justify-center h-[70%]">
            {file.contentType.startsWith("image/") ? (
              <img
                src={previewUrl}
                alt={file.fileName}
                className="max-w-full max-h-full object-contain"
              />
            ) : file.contentType === "application/pdf" ? (
              <iframe
                src={previewUrl}
                title="PDF Preview"
                className="w-full h-full"
              />
            ) : (
              <p>Preview not supported for this file type.</p>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default ViewFileModal;
