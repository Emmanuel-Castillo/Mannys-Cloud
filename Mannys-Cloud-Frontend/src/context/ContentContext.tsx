import React, { createContext, useContext, useState } from "react";
import type { Content } from "../types/ContentTypes";

interface ContentContextType {
  selectedContent: Content;
  setSelectedContent: React.Dispatch<React.SetStateAction<Content>>;
}

export const ContentContext = createContext<ContentContextType | undefined>(
  undefined
);

export const ContentProvider = ({
  children,
}: {
  children: React.ReactElement;
}) => {
  const [selectedContent, setSelectedContent] = useState<Content>({
    fileIds: [],
    folderIds: [],
  });

  const value: ContentContextType = {
    selectedContent,
    setSelectedContent,
  };

  return (
    <ContentContext.Provider value={value}>{children}</ContentContext.Provider>
  );
};

export const useContent = () => {
  const context = useContext(ContentContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an ContentContextProvider");
  }
  return context;
};
