import type { AxiosStatic } from "axios";
import axios from "axios";
import React, { createContext, useContext, useEffect, useState } from "react";
import toast from "react-hot-toast";
import type { AuthCredentials, AuthState } from "../types/AuthTypes";

axios.defaults.baseURL = import.meta.env.VITE_BACKEND_URL;

type AuthUser = {
  userId: number;
  fullName: string;
  email: string;
};

interface AuthContextType {
  axios: AxiosStatic;
  authUser: AuthUser | null;
  login: (state: AuthState, credentials: AuthCredentials    ) => void;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);

export const AuthProvider = ({
  children,
}: {
  children: React.ReactElement;
}) => {
  const [authUser, setAuthUser] = useState<AuthUser | null>(null);
  const [token, setToken] = useState(localStorage.getItem("token"));

  // Request server for AuthUser response
  const checkAuth = async () => {
    try {
      const { data } = await axios.get("/api/auth/check");
      if (data.success) {
        setAuthUser(data.userData);
      }
    } catch (error: any) {
      console.log(error)
      toast.error(error.message);
    }
  };

  const login = async (
    state: AuthState,
    credentials: AuthCredentials
  ) => {
    const toastId = toast.loading("Loading...")
    try {
      const { data } = await axios.post(`/api/auth/${state}`, credentials);
      if (data.success) {
        axios.defaults.headers.common["Authorization"] = `Bearer ${data.token}`;
        localStorage.setItem("token", data.token);
        toast.success(data.message, {id: toastId});
        setToken(data.token);
        setAuthUser(data.userData);
      }
    } catch (error: any) {
      toast.error(error.message, {id: toastId});
    }
  };
  
  const logout = async () => {
    axios.defaults.headers.common["Authorization"] = null;
    localStorage.removeItem("token")
    toast.success("Logged out successfully.");
    setToken(null);
    setAuthUser(null);
  };

  // If token is set, assign it to axios default headers
  // Afterwards, check authentication to receive AuthUser resposne
  useEffect(() => {
    if (token) {
      axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      checkAuth();
    }
  }, []);

  const value: AuthContextType = {
    axios,
    authUser,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthContextProvider");
  }
  return context;
};
