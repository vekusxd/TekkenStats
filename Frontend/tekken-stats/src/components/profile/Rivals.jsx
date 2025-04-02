import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { BASE_URL } from '../../config/baseUrl';
import styles from '../TekkenStatsProfile.module.css';
import { Link, useNavigate } from 'react-router-dom';
import Filters from './Filters';

const Rivals = ({ tekkenId, filters, setFilters, profile, opponentCharacters }) => {
  const [rivals, setRivals] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [profiles, setProfiles] = useState({});
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRivals = async () => {
      try {
        setLoading(true);
        
        const params = {};
        if (filters.playerCharacterId) params.PlayerCharacterId = filters.playerCharacterId;
        if (filters.opponentCharacterId) params.OpponentCharacterId = filters.opponentCharacterId;
        
        const response = await axios.get(`${BASE_URL}/api/rivals/${tekkenId}`, { params });
        const rivalsData = response.data.data;

        const profilesData = {};
        await Promise.all(
          rivalsData.map(async rival => {
            try {
              const profileResponse = await axios.get(`${BASE_URL}/api/profile/${rival.tekkenId}`);
              profilesData[rival.tekkenId] = profileResponse.data;
            } catch (err) {
              console.error(`Error loading profile for ${rival.tekkenId}:`, err);
              profilesData[rival.tekkenId] = {
                currentName: rival.name,
              };
            }
          })
        );

        setProfiles(profilesData);
        setRivals(rivalsData);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchRivals();
  }, [tekkenId, filters]);

  const handleRivalClick = (e, rivalTekkenId) => {
    e.preventDefault();
    navigate(`/head-to-head/${tekkenId}/${rivalTekkenId}`);
  };

  if (loading) return <div className={styles.loading}>Loading rivals...</div>;
  if (error) return <div className={styles.error}>Error loading rivals: {error}</div>;

  return (
    <div className={styles.tabContent}>
      <Filters 
        filters={filters} 
        setFilters={setFilters} 
        profile={profile} 
        opponentCharacters={opponentCharacters}
      />

      <div className={styles.rivalsList}>
        {rivals.length > 0 ? (
          rivals.map(rival => {
            const profile = profiles[rival.tekkenId] || {};
            return (
              <Link 
                key={rival.tekkenId} 
                to={`/head-to-head/${tekkenId}/${rival.tekkenId}`}
                className={styles.rivalLink}
                onClick={(e) => handleRivalClick(e, rival.tekkenId)}
              >
                <div className={styles.matchItem}>
                  <div className={styles.rivalInfo}>
                    <div className={styles.rivalTextInfo}>
                      <p className={styles.opponentName}>{profile.currentName || rival.name}</p>
                      <div className={styles.matchDetails}>
                        <span className={styles.winRate}>{rival.winRate}% WR</span>
                      </div>
                    </div>
                  </div>
                  <div className={styles.rivalStats}>
                    <span className={styles.textGray}>Matches: {rival.totalMatches}</span>
                  </div>
                </div>
              </Link>
            );
          })
        ) : (
          <div className={styles.noRivals}>No rivals found</div>
        )}
      </div>
    </div>
  );
};

export default Rivals;