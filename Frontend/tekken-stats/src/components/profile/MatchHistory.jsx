import React from 'react';
import { useNavigate } from 'react-router-dom';
import styles from '../TekkenStatsProfile.module.css';
import Filters from './Filters';

const MatchHistory = ({ matches, filters, setFilters, profile, opponentCharacters }) => {
  return (
    <div className={styles.tabContent}>
      <Filters 
        filters={filters} 
        setFilters={setFilters} 
        profile={profile} 
        opponentCharacters={opponentCharacters}
      />
      
      <div className={styles.matchesList}>
        {matches.map(match => (
          <MatchItem key={match.battleId} match={match} />
        ))}
        <button 
          className={styles.loadMoreButton}
          onClick={() => setFilters({...filters, pageSize: filters.pageSize + 10})}
        >
          Load More Matches
        </button>
      </div>
    </div>
  );
};

const MatchItem = ({ match }) => {
  const navigate = useNavigate();
  
  const challengerImg = `http://localhost:8080/${match.challenger.characterImgURL}`;
  const opponentImg = `http://localhost:8080/${match.opponent.characterImgURL}`;

  const handlePlayerClick = (playerId) => {
    navigate(`/${playerId}`);
  };

  return (
    <div className={styles.matchItem}>
      <div className={styles.matchInfo}>
        <div className={styles.characterImages}>
          <div className={styles.characterImageContainer}>
            <img 
              src={challengerImg} 
              alt={match.challenger.characterName}
              className={styles.characterImage}
              onError={(e) => {
                e.target.onerror = null;
                e.target.src = '/images/default-character.png';
              }}
            />
          </div>
          <span className={styles.vsText}>VS</span>
          <div 
            className={styles.characterImageContainer}
            onClick={() => handlePlayerClick(match.opponent.tekkenId)}
            style={{ cursor: 'pointer' }}
          >
            <img 
              src={opponentImg} 
              alt={match.opponent.characterName}
              className={styles.characterImage}
              onError={(e) => {
                e.target.onerror = null;
                e.target.src = '/images/default-character.png';
              }}
            />
          </div>
        </div>
        <div className={styles.matchDetailsContainer}>
          <p 
            className={styles.opponentName}
            onClick={() => handlePlayerClick(match.opponent.tekkenId)}
            style={{ cursor: 'pointer' }}
          >
            {match.opponent.name}
          </p>
          <div className={styles.matchDetails}>
            <span className={styles.textGray}>
              {match.challenger.characterName} vs {match.opponent.characterName}
            </span>
            <span className={styles.textGray}>•</span>
            <span className={styles.textGray}>
              {match.challenger.rounds}-{match.opponent.rounds}
            </span>
            <span className={`${match.winner ? styles.textGreen : styles.textRed} ${styles.ratingChange}`}>
              {match.winner ? '+' : '-'}{Math.abs(match.challenger.ratingChange)}
            </span>
          </div>
        </div>
      </div>
      <p className={`${match.winner ? styles.textGreen : styles.textRed} ${styles.matchResult}`}>
        {match.winner ? 'Victory' : 'Defeat'}
      </p>
    </div>
  );
};

export default MatchHistory;