import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import './App.css'
import { Header } from './components/Header/Header'
import { ClientsPage } from './pages/ClientsPage/ClientsPage'
import { SettingsPage } from './pages/SettingsPage/SettingsPage'

function App() {
  return (<>
    <BrowserRouter>
      <div className="flex flex-col h-screen">
        <Header />

        <main className="flex-1 min-h-0 overflow-hidden">
          <Routes>
            <Route path="/" element={<Navigate to="/clients" replace />} />
            <Route path="/clients" element={<ClientsPage />} />
            <Route path="/settings" element={<SettingsPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  </>)
}

export default App
