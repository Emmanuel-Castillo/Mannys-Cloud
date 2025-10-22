import { Navigate, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "./context/AuthContext";
import Navbar from "./components/Navbar";
import ActionSpan from "./components/ActionSpan";

const Layout = () => {
  const { authUser } = useAuth();
  const navigate = useNavigate();

  if (!authUser) return <Navigate to={"/login"} replace />;
  return (
    <div className="h-screen w-full p-12 flex flex-col gap-4 md:gap-0">
      <Navbar />
      <div className="w-full h-full md:grid md:grid-cols-[1fr_3fr] lg:grid-cols-[1fr_4fr] xl:grid-cols-[1fr_6fr]">
        {/* page navigation */}
        <div className="bg-gray-700 p-4 flex flex-col">
          <ActionSpan
            text="Home"
            onClickButton={() => {
              navigate(0);
            }}
          />
          <ActionSpan text="Trash" onClickButton={() => {navigate("/trash")}} />
        </div>
        <Outlet />
      </div>
    </div>
  );
};

export default Layout;
