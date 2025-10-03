import React, { useState } from "react";
import type { AuthCredentials, AuthState } from "../types/AuthTypes";
import { useAuth } from "../context/AuthContext";

const LoginPage = () => {
  const [authState, setAuthState] = useState<AuthState>("login");
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const { login } = useAuth()

  const formHandler = () => {
    const authCredentials: AuthCredentials = {
      fullName,
      email,
      password,
    };

    login(authState, authCredentials)
  };
  
  return (
    <div className="min-h-screen flex items-center justify-center">
      <form
        onSubmit={(e) => {
          e.preventDefault();
          formHandler();
        }}
        className="border-2 bg-white/8 text-white border-gray-500 p-6 flex flex-col gap-6 rounded-lg shadow-lg"
      >
        <h2 className="font-medium text-2xl flex justify-between items-center">
          {authState === "login" ? "Login" : "Register"}
        </h2>
        {authState === "register" && (
          <input
            type="text"
            className="p-2 border border-gray-500 rounded-md focus:outine-none"
            placeholder="Full Name"
            required
            onChange={(e) => setFullName(e.target.value)}
            value={fullName}
          />
        )}

        <input
          type="email"
          placeholder="Email address"
          required
          className="p-2 border border-gray-500 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
          onChange={(e) => setEmail(e.target.value)}
          value={email}
        />
        <input
          type="password"
          placeholder="Password"
          required
          className="p-2 border border-gray-500 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
          onChange={(e) => setPassword(e.target.value)}
          value={password}
        />

        <button className="py-3 bg-gradient-to-r from-purple-400 to-violet-600 text-white rounded-md cursor-pointer">
          {authState === "register" ? "Register" : "Login"}
        </button>

        <div className="flex flex-col gap-2">
          {authState === "register" ? (
            <p className="text-sm text-gray-600">
              Already have an account?{" "}
              <span
                className="font-medium text-violet-500 cursor-pointer"
                onClick={() => {
                  setAuthState("login");
                }}
              >
                Login here
              </span>
            </p>
          ) : (
            <p className="text-sm text-gray-600">
              Register an account{" "}
              <span
                className="font-medium text-violet-500 cursor-pointer"
                onClick={() => setAuthState("register")}
              >
                Click here
              </span>
            </p>
          )}
        </div>
      </form>
    </div>
  );
};

export default LoginPage;
