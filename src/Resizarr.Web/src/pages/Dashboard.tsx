import { useEffect, useState } from 'react';
import { dashboardApi, type DashboardStats } from '../api/api';

export default function Dashboard() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const response = await dashboardApi.getStats();
        setStats(response.data);
        setError(null);
      } catch (err) {
        setError('Failed to load dashboard statistics');
        console.error('Error fetching dashboard stats:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
    const interval = setInterval(fetchStats, 5000); // Refresh every 5 seconds

    return () => clearInterval(interval);
  }, []);

  if (loading) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>Dashboard</h1>
        <p>Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>Dashboard</h1>
        <p style={{ color: 'red' }}>{error}</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px' }}>
      <h1>Resizarr Dashboard</h1>
      
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '20px', marginTop: '20px' }}>
        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
          <h3>Total Media Files</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.totalMediaFiles || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#fff3cd' }}>
          <h3>Pending Files</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.pendingFiles || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#d1ecf1' }}>
          <h3>Processing Files</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.processingFiles || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#d4edda' }}>
          <h3>Completed Files</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.completedFiles || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#f8d7da' }}>
          <h3>Failed Files</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.failedFiles || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#e2e3e5' }}>
          <h3>Active Jobs</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.activeJobs || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#e2e3e5' }}>
          <h3>Queued Jobs</h3>
          <p style={{ fontSize: '2em', margin: '10px 0' }}>{stats?.queuedJobs || 0}</p>
        </div>

        <div style={{ border: '1px solid #ddd', padding: '20px', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
          <h3>Bytes Processed</h3>
          <p style={{ fontSize: '1.5em', margin: '10px 0' }}>
            {((stats?.totalBytesProcessed || 0) / (1024 * 1024 * 1024)).toFixed(2)} GB
          </p>
        </div>
      </div>

      <div style={{ marginTop: '40px' }}>
        <h2>Quick Actions</h2>
        <div style={{ display: 'flex', gap: '10px', marginTop: '10px' }}>
          <button style={{ padding: '10px 20px', fontSize: '1em', cursor: 'pointer' }}>
            Scan Folders
          </button>
          <button style={{ padding: '10px 20px', fontSize: '1em', cursor: 'pointer' }}>
            View Queue
          </button>
        </div>
      </div>
    </div>
  );
}
