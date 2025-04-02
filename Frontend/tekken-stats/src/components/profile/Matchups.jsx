import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { BASE_URL } from '../../config/baseUrl';
import styles from '../TekkenStatsProfile.module.css';

const Matchups = ({ tekkenId, profile }) => {
  const [matchups, setMatchups] = useState([]);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({
    playerCharacterId: 'All Characters'
  });

  useEffect(() => {
    const fetchMatchups = async () => {
      try {
        const response = await axios.get(`${BASE_URL}/api/matchups/${tekkenId}`, {
          params: {
            playerCharacterId: filters.playerCharacterId === 'All Characters' ? undefined : filters.playerCharacterId
          }
        });
        setMatchups(response.data);
      } catch (err) {
        setError(err.message);
      }
    };

    fetchMatchups();
  }, [tekkenId, filters]);

  if (error) return <div className={styles.error}>Error loading matchups: {error}</div>;

  return (
    <div className={styles.tabContent}>
      <div className={styles.filtersContainer}>
        <div className={styles.filterItem}>
          <label className={styles.filterLabel}>Your Character</label>
          <select
            className={styles.selectInput}
            value={filters.playerCharacterId}
            onChange={(e) => setFilters({...filters, playerCharacterId: e.target.value})}
          >
            <option value="All Characters">All Characters</option>
            {(profile.characters || []).map(char => (
              <option key={char.characterId} value={char.characterId}>
                {char.characterName}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className={styles.matchupsList}>
        {matchups.length > 0 ? (
          matchups.map(matchup => {
            const characterImageUrl = matchup.characterImgURL 
              ? `${BASE_URL}/${matchup.characterImgURL.replace(/^\/+/, '')}`
              : '/images/default-character.png';

            return (
              <div key={matchup.opponentCharacterId} className={styles.matchupItem}>
                <div className={styles.matchupInfo}>
                  <div className={styles.characterImageContainer}>
                    <img 
                      src={characterImageUrl}
                      alt={matchup.characterName}
                      className={styles.characterImage}
                      onError={(e) => {
                        e.target.onerror = null;
                        e.target.src = '/images/default-character.png';
                      }}
                    />
                  </div>
                  <div>
                    <p className={styles.opponentName}>{matchup.characterName}</p>
                    <div className={styles.matchDetails}>
                      <span className={styles.textGray}>
                        {matchup.wins}W - {matchup.losses}L
                      </span>
                      <span className={styles.textGray}>•</span>
                      <span className={styles.textGray}>
                        {matchup.totalMatches} matches
                      </span>
                    </div>
                  </div>
                </div>
                <div className={styles.matchupStats}>
                  <span className={styles.matchCount}>{matchup.winRate}% WR</span>
                  <div className={styles.winRateBar}>
                    <div 
                      className={styles.winRateFill}
                      style={{ width: `${matchup.winRate}%` }}
                    ></div>
                  </div>
                </div>
              </div>
            );
          })
        ) : (
          <div className={styles.noData}>No matchups found</div>
        )}
      </div>
    </div>
  );
};

export default Matchups;