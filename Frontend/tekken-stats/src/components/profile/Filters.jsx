import React from 'react';
import styles from "../../styles/TekkenStatsProfile.module.css";

const Filters = ({ 
  filters, 
  setFilters, 
  profile, 
  opponentCharacters,
  showPlayerCharacterFilter = true,
  showOpponentCharacterFilter = true,
  playerFilterKey = 'playerCharacterId'
}) => {
  const handleFilterChange = (filterName, value) => {
    const newFilters = {
      ...filters,
      [filterName]: value === 'All Characters' ? null : value,
      pageNumber: 1 
    };
    setFilters(newFilters);
  };

  return (
    <div className={styles.filtersContainer}>
      {showPlayerCharacterFilter && (
        <div className={styles.filterItem}>
          <label className={styles.filterLabel}>Your Character</label>
          <select 
            className={styles.selectInput}
            value={filters[playerFilterKey] || 'All Characters'}
            onChange={(e) => handleFilterChange(playerFilterKey, e.target.value)}
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