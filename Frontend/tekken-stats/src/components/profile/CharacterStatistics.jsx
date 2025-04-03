import React from 'react';
import styles from '../../styles/CharacterStatistics.module.css';

const CharacterStatistics = ({ characters = [] }) => {
  return (
    <div className={styles.sidebarSection}>
      <h2 className={styles.sectionTitle}>Character Statistics</h2>
      <div className={styles.charactersList}>
        {characters.map(character => (
          <CharacterItem key={character.characterId} character={character} />
        ))}
      </div>
    </div>
  );
};

const CharacterItem = ({ character }) => {
  const characterImageUrl = character.imgURL 
    ? `http://localhost:8080/${character.imgURL.replace(/^\/+/, '')}`
    : '/images/default-character.png';

  const formatUTCDate = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return `${date.getUTCDate()} ${date.toLocaleString('en-US', { month: 'short'})} ${date.getUTCFullYear()}`;
  };

  const getWinRateColor = (winRate) => {
    if (winRate >= 50) return '#10B981'; // g
    return '#EF4444'; // r
  };

  const winRate = Math.round((character.winCount / character.matchesCount) * 100);
  const winRateColor = getWinRateColor(winRate);

  return (
    <div className={styles.characterCard}>
      <div className={styles.characterHeader}>
        <div className={styles.characterAvatar}>
          <img 
            src={characterImageUrl}
            alt={character.characterName}
            className={styles.characterImage}
            onError={(e) => {
              e.target.onerror = null;
              e.target.src = '/images/default-character.png';
            }}
          />
        </div>
        <div className={styles.characterTitle}>
          <h3 className={styles.characterName}>{character.characterName}</h3>
          <div className={styles.characterMeta}>
            <span className={styles.characterMatches}>{character.matchesCount} matches</span>
            {character.rating && (
              <span className={styles.characterRating}>
                {character.rating} RP
              </span>
            )}
          </div>
        </div>
      </div>
      
      <div className={styles.characterProgress}>
        <div className={styles.progressBar}>
          <div 
            className={styles.progressFill}
            style={{ 
              width: `${winRate}%`,
              backgroundColor: winRateColor
            }}
          ></div>
        </div>
        <div className={styles.progressStats}>
          <div className={styles.winRateGroup}>
            <span className={styles.winRateLabel}>Win Rate:</span>
            <span className={styles.winRateValue} style={{ color: winRateColor }}>
              {winRate}%
            </span>
          </div>
          <span className={styles.winLoss}>
            {character.winCount}W / {character.matchesCount - character.winCount}L
          </span>
        </div>
      </div>

      {character.lastPlayed && (
        <div className={styles.lastPlayed}>
          <svg className={styles.calendarIcon} viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          Last played: {formatUTCDate(character.lastPlayed)}
        </div>
      )}
    </div>
  );
};

export default CharacterStatistics;