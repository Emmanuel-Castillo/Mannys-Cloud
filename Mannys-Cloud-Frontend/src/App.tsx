import { Toaster } from 'react-hot-toast'
import { Navigate, Route, Routes } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import Layout from './Layout'
import TrashPage from './pages/TrashPage'

function App() {

  const { authUser } = useAuth()
  return (
    <div className=' bg-gray-950 text-white'>
      <Toaster/>
      <Routes>
        <Route element={<Layout/>}>
          <Route path='/' element={authUser ? <HomePage/> : <Navigate to={"/login"}/>}/>
          <Route path='/trash' element={authUser ? <TrashPage/> : <Navigate to={"/login"}/>}/>
        </Route>
        <Route path='/login' element={!authUser ? <LoginPage/> : <Navigate to={"/"}/>}/>
      </Routes>
    </div>
  )
}

export default App
