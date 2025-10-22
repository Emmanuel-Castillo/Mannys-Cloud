import { useAuth } from "../context/AuthContext";

const Navbar = () => {
  const { authUser, logout } = useAuth();
  return authUser && (
    <div className="flex justify-between items-center">
      <h1>Welcome, {authUser.fullName}</h1>
      <button
        onClick={logout}
        className="p-3 bg-gradient-to-r from-red-400 to-red-600 rounded-md cursor-pointer"
      >
        Logout
      </button>
    </div>
  );
};

export default Navbar;
