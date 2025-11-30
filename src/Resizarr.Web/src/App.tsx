import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import QualityProfiles from './pages/QualityProfiles';
import ActivityQueue from './pages/ActivityQueue';
import './App.css';

function App() {
  return (
    <Router>
      <div style={{ display: 'flex', minHeight: '100vh' }}>
        {/* Sidebar Navigation */}
        <nav style={{
          width: '250px',
          backgroundColor: '#2c3e50',
          color: 'white',
          padding: '20px',
          boxShadow: '2px 0 5px rgba(0,0,0,0.1)'
        }}>
          <h2 style={{ marginBottom: '30px', fontSize: '1.5em' }}>Resizarr</h2>
          <ul style={{ listStyle: 'none', padding: 0 }}>
            <li style={{ marginBottom: '10px' }}>
              <Link to="/" style={{ color: 'white', textDecoration: 'none', display: 'block', padding: '10px', borderRadius: '4px' }}>
                📊 Dashboard
              </Link>
            </li>
            <li style={{ marginBottom: '10px' }}>
              <Link to="/quality-profiles" style={{ color: 'white', textDecoration: 'none', display: 'block', padding: '10px', borderRadius: '4px' }}>
                ⚙️ Quality Profiles
              </Link>
            </li>
            <li style={{ marginBottom: '10px' }}>
              <Link to="/activity-queue" style={{ color: 'white', textDecoration: 'none', display: 'block', padding: '10px', borderRadius: '4px' }}>
                📋 Activity Queue
              </Link>
            </li>
            <li style={{ marginBottom: '10px' }}>
              <Link to="/media" style={{ color: 'white', textDecoration: 'none', display: 'block', padding: '10px', borderRadius: '4px' }}>
                🎬 Media Files
              </Link>
            </li>
            <li style={{ marginBottom: '10px' }}>
              <Link to="/folders" style={{ color: 'white', textDecoration: 'none', display: 'block', padding: '10px', borderRadius: '4px' }}>
                📁 Folders
              </Link>
            </li>
            <li style={{ marginBottom: '10px' }}>
              <Link to="/settings" style={{ color: 'white', textDecoration: 'none', display: 'block', padding: '10px', borderRadius: '4px' }}>
                🔧 Settings
              </Link>
            </li>
          </ul>
        </nav>

        {/* Main Content */}
        <main style={{ flex: 1, backgroundColor: '#f8f9fa' }}>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/quality-profiles" element={<QualityProfiles />} />
            <Route path="/activity-queue" element={<ActivityQueue />} />
            <Route path="/media" element={<div style={{ padding: '20px' }}><h1>Media Files</h1><p>Coming soon...</p></div>} />
            <Route path="/folders" element={<div style={{ padding: '20px' }}><h1>Folders</h1><p>Coming soon...</p></div>} />
            <Route path="/settings" element={<div style={{ padding: '20px' }}><h1>Settings</h1><p>Coming soon...</p></div>} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
