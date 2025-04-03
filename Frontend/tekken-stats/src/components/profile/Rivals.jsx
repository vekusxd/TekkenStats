import React from 'react';
import styles from '../../styles/Rivals.module.css';
import { Link, useNavigate } from 'react-router-dom';
import Filters from './Filters';
import { getWinRateColor } from '../../styles/winRateStyles';

const Rivals = ({ 
  rivals, 
  rivalsProfiles, 
  filters, 
  setFilters, 
  profile, 
  opponentCharacters,
  tekkenId 
}) => {
  const navigate = useNavigate();

  const handleRivalClick = (e, rivalTekkenId) => {
    e.preventDefault();
    navigate(`/head-to-head/${tekkenId}/${rivalTekkenId}`);
  };

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
            const profile = rivalsProfiles[rival.tekkenId] || {};
            const winRateColor = getWinRateColor(rival.winRate);
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
                      <span className={styles.winRateLabel}>Win Rate: </span>
                      <span style={{ color: winRateColor }}>
                        {rival.winRate}%
                      </span>
                      </div>
                    </div>
                  </div>
                  <div className={styles.rivalStats}>
                    <span className={styles.textF}>Matches: {rival.totalMatches}</span>
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