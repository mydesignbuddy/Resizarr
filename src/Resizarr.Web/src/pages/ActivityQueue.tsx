import { useEffect, useState } from 'react';
import { jobsApi, type TranscodingJob } from '../api/api';

export default function ActivityQueue() {
  const [jobs, setJobs] = useState<TranscodingJob[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<string>('');

  useEffect(() => {
    const fetchJobs = async () => {
      try {
        const response = await jobsApi.getAll(filter || undefined);
        setJobs(response.data);
        setError(null);
      } catch (err) {
        setError('Failed to load jobs');
        console.error('Error fetching jobs:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchJobs();
    const interval = setInterval(fetchJobs, 3000); // Refresh every 3 seconds

    return () => clearInterval(interval);
  }, [filter]);

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'running': return '#0d6efd';
      case 'completed': return '#28a745';
      case 'failed': return '#dc3545';
      case 'queued': return '#6c757d';
      case 'cancelled': return '#ffc107';
      default: return '#6c757d';
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>Activity Queue</h1>
        <p>Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>Activity Queue</h1>
        <p style={{ color: 'red' }}>{error}</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px' }}>
      <h1>Activity Queue</h1>

      <div style={{ marginTop: '20px', marginBottom: '20px' }}>
        <label htmlFor="filter">Filter by status: </label>
        <select
          id="filter"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          style={{ marginLeft: '10px', padding: '5px 10px' }}
        >
          <option value="">All</option>
          <option value="Queued">Queued</option>
          <option value="Running">Running</option>
          <option value="Completed">Completed</option>
          <option value="Failed">Failed</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </div>

      {jobs.length === 0 ? (
        <div style={{ marginTop: '20px', padding: '40px', border: '2px dashed #ddd', borderRadius: '8px', textAlign: 'center' }}>
          <p>No transcoding jobs found.</p>
        </div>
      ) : (
        <table style={{ width: '100%', marginTop: '20px', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ backgroundColor: '#f5f5f5', borderBottom: '2px solid #ddd' }}>
              <th style={{ padding: '12px', textAlign: 'left' }}>Media File</th>
              <th style={{ padding: '12px', textAlign: 'left' }}>Profile</th>
              <th style={{ padding: '12px', textAlign: 'center' }}>Status</th>
              <th style={{ padding: '12px', textAlign: 'center' }}>Progress</th>
              <th style={{ padding: '12px', textAlign: 'left' }}>Queued At</th>
              <th style={{ padding: '12px', textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {jobs.map((job) => (
              <tr key={job.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: '12px' }}>
                  <strong>{job.mediaFileName}</strong>
                </td>
                <td style={{ padding: '12px' }}>{job.qualityProfileName}</td>
                <td style={{ padding: '12px', textAlign: 'center' }}>
                  <span style={{
                    padding: '4px 12px',
                    borderRadius: '12px',
                    fontSize: '0.85em',
                    backgroundColor: getStatusColor(job.status) + '20',
                    color: getStatusColor(job.status),
                    fontWeight: 'bold'
                  }}>
                    {job.status}
                  </span>
                </td>
                <td style={{ padding: '12px', textAlign: 'center' }}>
                  {job.progress > 0 ? (
                    <div>
                      <div style={{
                        width: '100px',
                        height: '8px',
                        backgroundColor: '#e9ecef',
                        borderRadius: '4px',
                        overflow: 'hidden',
                        display: 'inline-block'
                      }}>
                        <div style={{
                          width: `${job.progress}%`,
                          height: '100%',
                          backgroundColor: getStatusColor(job.status),
                          transition: 'width 0.3s ease'
                        }} />
                      </div>
                      <span style={{ marginLeft: '10px' }}>{job.progress.toFixed(1)}%</span>
                    </div>
                  ) : (
                    <span>-</span>
                  )}
                </td>
                <td style={{ padding: '12px' }}>
                  {new Date(job.queuedAt).toLocaleString()}
                </td>
                <td style={{ padding: '12px', textAlign: 'right' }}>
                  <button style={{ padding: '5px 10px', cursor: 'pointer' }}>
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
