import { useEffect, useState } from 'react';
import { qualityProfilesApi, type QualityProfile } from '../api/api';

export default function QualityProfiles() {
  const [profiles, setProfiles] = useState<QualityProfile[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const response = await qualityProfilesApi.getAll();
        setProfiles(response.data);
        setError(null);
      } catch (err) {
        setError('Failed to load quality profiles');
        console.error('Error fetching quality profiles:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchProfiles();
  }, []);

  if (loading) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>Quality Profiles</h1>
        <p>Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ padding: '20px' }}>
        <h1>Quality Profiles</h1>
        <p style={{ color: 'red' }}>{error}</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1>Quality Profiles</h1>
        <button style={{ padding: '10px 20px', fontSize: '1em', cursor: 'pointer' }}>
          Add New Profile
        </button>
      </div>

      {profiles.length === 0 ? (
        <div style={{ marginTop: '20px', padding: '40px', border: '2px dashed #ddd', borderRadius: '8px', textAlign: 'center' }}>
          <p>No quality profiles configured yet.</p>
          <p>Create your first profile to start transcoding media files.</p>
        </div>
      ) : (
        <table style={{ width: '100%', marginTop: '20px', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ backgroundColor: '#f5f5f5', borderBottom: '2px solid #ddd' }}>
              <th style={{ padding: '12px', textAlign: 'left' }}>Name</th>
              <th style={{ padding: '12px', textAlign: 'left' }}>Resolution</th>
              <th style={{ padding: '12px', textAlign: 'left' }}>Codec</th>
              <th style={{ padding: '12px', textAlign: 'left' }}>Hardware Accel</th>
              <th style={{ padding: '12px', textAlign: 'center' }}>Status</th>
              <th style={{ padding: '12px', textAlign: 'right' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {profiles.map((profile) => (
              <tr key={profile.id} style={{ borderBottom: '1px solid #eee' }}>
                <td style={{ padding: '12px' }}>
                  <strong>{profile.name}</strong>
                  {profile.description && (
                    <div style={{ fontSize: '0.9em', color: '#666' }}>{profile.description}</div>
                  )}
                </td>
                <td style={{ padding: '12px' }}>{profile.targetResolution}</td>
                <td style={{ padding: '12px' }}>{profile.targetCodec}</td>
                <td style={{ padding: '12px' }}>{profile.hardwareAcceleration}</td>
                <td style={{ padding: '12px', textAlign: 'center' }}>
                  <span style={{
                    padding: '4px 12px',
                    borderRadius: '12px',
                    fontSize: '0.85em',
                    backgroundColor: profile.enabled ? '#d4edda' : '#f8d7da',
                    color: profile.enabled ? '#155724' : '#721c24'
                  }}>
                    {profile.enabled ? 'Enabled' : 'Disabled'}
                  </span>
                </td>
                <td style={{ padding: '12px', textAlign: 'right' }}>
                  <button style={{ marginLeft: '5px', padding: '5px 10px', cursor: 'pointer' }}>
                    Edit
                  </button>
                  <button style={{ marginLeft: '5px', padding: '5px 10px', cursor: 'pointer', color: 'red' }}>
                    Delete
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
