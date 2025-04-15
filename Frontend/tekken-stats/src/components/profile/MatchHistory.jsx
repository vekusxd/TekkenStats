import React from 'react';
import { Link } from 'react-router-dom';
import styles from '../../styles/MatchHistory.module.css';
import Filters from './Filters';
import { BASE_URL } from '../../config/baseUrl';

const MatchHistory = ({ matches, totalMatches, filters, setFilters, profile, opponentCharacters }) => {

  const handleLoadMore = () => {
    setFilters(prev => ({
      ...prev,
      pageNumber: prev.pageNumber + 1
    }));
  };

  const hasMoreMatches = matches.length < totalMatches;

  return (
    <div className={styles.tabContent}>
      <Filters 
        filters={filters} 
        setFilters={(newFilters) => setFilters({...newFilters, pageNumber: 1})}
        profile={profile} 
        playerFilterKey="characterId"
        opponentCharacters={opponentCharacters}
      />
      
      <div className={styles.matchesList}>
        {matches.map(match => (
          <MatchItem key={match.battleId} match={match} />
        ))}
        {hasMoreMatches && (
          <button 
            className={styles.loadMoreButton}
            onClick={handleLoadMore}
          >
            Load More Matches
          </button>
        )}
      </div>
    </div>
  );
};

const MatchItem = ({ match }) => {
  const challengerImg = `${BASE_URL}/${match.challenger.characterImgURL}`;
  const opponentImg = `${BASE_URL}/${match.opponent.characterImgURL}`;

  const formatUTCDate = (dateString) => {
    const date = new Date(dateString);
    const day = date.getUTCDate();
    const month = date.toLocaleString('en-US', { month: 'short'});
    const year = date.getUTCFullYear();
    return `${day} ${month} ${year}`;
  };

  const formatUTCTime = (dateString) => {
    const date = new Date(dateString);
    const hours = date.getUTCHours().toString().padStart(2, '0');
    const minutes = date.getUTCMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  };

  const formattedDate = formatUTCDate(match.date);
  const formattedTime = formatUTCTime(match.date);

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
          <Link 
            to={`/player/${match.opponent.tekkenId}`}
            className={styles.characterImageContainer}
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
          </Link>
        </div>
        <div className={styles.matchDetailsContainer}>
          <Link 
            to={`/player/${match.opponent.tekkenId}`}
            className={styles.opponentName}
          >
            {match.opponent.name}
          </Link>
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
          <div className={styles.matchTime}>
            <span className={styles.textGray}>{formattedDate}</span>
            <span className={styles.textGray}>•</span>
            <span className={styles.textGray}>{formattedTime}</span>
          </div>
        </div>
      </div>
      <div className={styles.matchResultContainer}>
        <p className={`${match.winner ? styles.textGreen : styles.textRed} ${styles.matchResult}`}>
          {match.winner ? 'Victory' : 'Defeat'}
        </p>
      </div>
    </div>
  );
};

export default MatchHistory;