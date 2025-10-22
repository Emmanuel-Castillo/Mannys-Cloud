import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext.tsx";
import { ContentProvider } from "./context/ContentContext.tsx";

createRoot(document.getElementById("root")!).render(
  <BrowserRouter>
    <AuthProvider>
      <ContentProvider>
        <App />
      </ContentProvider>
    </AuthProvider>
  </BrowserRouter>
);
