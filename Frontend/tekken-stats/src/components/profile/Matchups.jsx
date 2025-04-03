import React, { useState } from 'react';
import styles from '../../styles/Matchups.module.css';
import { getWinRateColor } from '../../styles/winRateStyles';
import { useMatchups } from '../../hooks/useMatchups';
import Filters from './Filters';
import { BASE_URL } from '../../config/baseUrl';

const Matchups = ({ tekkenId, profile }) => {
  const [filters, setFilters] = useState({
    playerCharacterId: 'All Characters'
  });

  const { matchups, loading, error } = useMatchups(tekkenId, filters.playerCharacterId);

  if (loading) return <div className={styles.loading}>Loading matchups...</div>;
  if (error) return <div className={styles.error}>Error loading matchups: {error}</div>;

  return (
    <div className={styles.tabContent}>
      <Filters 
        filters={filters}
        setFilters={setFilters}
        profile={profile}
        showOpponentCharacterFilter={false}
      />

      <div className={styles.matchupsList}>
        {matchups.length > 0 ? (
          matchups.map(matchup => {
            const characterImageUrl = matchup.characterImgURL 
              ? `${BASE_URL}/${matchup.characterImgURL.replace(/^\/+/, '')}`
              : '/images/default-character.png';

            const winRateColor = getWinRateColor(matchup.winRate);

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
                  <div className={styles.winRateGroup}>
                    <span className={styles.matchCountLabel}>Win Rate: </span>
                    <span style={{ color: winRateColor }}>
                      {matchup.winRate}%
                    </span>
                  </div>
                  <div className={styles.winRateBar}>
                    <div 
                      className={styles.winRateFill}
                      style={{ 
                        width: `${matchup.winRate}%`,
                        backgroundColor: winRateColor
                      }}
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