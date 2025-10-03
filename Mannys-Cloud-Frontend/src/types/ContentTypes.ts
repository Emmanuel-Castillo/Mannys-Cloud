export type FileDto = {
    fileId: number;
    userId: number;
    fileName: string;
    blobPath: string;
    contentType: string;
    sizeBytes: number;
    folderId: number | null;
    createdAt: string;
    isDeleted: boolean
}

export type FolderDto = {
    folderId: number;
    userId: number;
    folderName: string;
    isRootFolder: boolean;
    parentFolderId: number;
    createdAt: string;
    isDeleted: boolean;
    childFolders: FolderDto[]
    files: FileDto[]
}