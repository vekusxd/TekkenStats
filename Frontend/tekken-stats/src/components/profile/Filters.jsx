import React from 'react';
import styles from '../TekkenStatsProfile.module.css';

const Filters = ({ 
  filters, 
  setFilters, 
  profile, 
  opponentCharacters,
  showPlayerCharacterFilter = true,
  showOpponentCharacterFilter = true
}) => {
  const handleFilterChange = (filterName, value) => {
    setFilters(prev => ({
      ...prev,
      [filterName]: value === 'All Characters' ? null : value
    }));
  };

  return (
    <div className={styles.filtersContainer}>
      {showPlayerCharacterFilter && (
        <div className={styles.filterItem}>
          <label className={styles.filterLabel}>Your Character</label>
          <select 
            className={styles.selectInput}
            value={filters.playerCharacterId || 'All Characters'}
            onChange={(e) => handleFilterChange('playerCharacterId', e.target.value)}
          >
            <option value="All Characters">All Characters</option>
            {(profile.characters || []).map(char => (
              <option key={char.characterId} value={char.characterId}>
                {char.characterName}
              </option>
            ))}
          </select>
        </div>
      )}
      
      {showOpponentCharacterFilter && (
        <div className={styles.filterItem}>
          <label className={styles.filterLabel}>Opponent Character</label>
          <select 
            className={styles.selectInput}
            value={filters.opponentCharacterId || 'All Characters'}
            onChange={(e) => handleFilterChange('opponentCharacterId', e.target.value)}
          >
            <option value="All Characters">All Characters</option>
            {opponentCharacters.map(char => (
              <option key={char.characterId} value={char.characterId}>
                {char.characterName}
              </option>
            ))}
          </select>
        </div>
      )}
    </div>
  );
};

export default Filters;