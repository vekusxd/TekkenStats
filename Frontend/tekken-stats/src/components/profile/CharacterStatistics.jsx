import React from 'react';
import styles from '../TekkenStatsProfile.module.css';

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

  return (
    <div className={styles.characterStatItem}>
      <div className={styles.characterInfo}>
        <div className={styles.characterImageContainer}>
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
        <div>
          <p className={styles.characterName}>{character.characterName}</p>
          <p className={styles.characterWinRate}>
            Win Rate: {Math.round((character.winCount / character.matchesCount) * 100)}%
          </p>
        </div>
      </div>
      <div className={styles.characterStats}>
        <p className={styles.matchesCount}>{character.matchesCount} matches</p>
        <p className={styles.lastPlayed}>
          {character.lastPlayed ? `Last played: ${new Date(character.lastPlayed).toLocaleDateString()}` : ''}
        </p>
      </div>
    </div>
  );
};

export default CharacterStatistics;