import React from 'react';
import styles from '../styles/HeadToHead.module.css';
import Header from './Header';
import { useHeadToHeadData } from '../hooks/useHeadToHeadData';

const HeadToHead = () => {
  const {
    loading,
    error,
    headToHeadData,
    filteredMatches,
    winCount,
    lossCount,
    winRate,
    playerName,
    opponentName,
    playerRating,
    opponentRating,
    yourCharacters,
    opponentCharacters,
    yourCharacterFilter,
    setYourCharacterFilter,
    opponentCharacterFilter,
    setOpponentCharacterFilter,
    resultFilter,
    setResultFilter
  } = useHeadToHeadData();

  if (loading) return <div className={styles.loading}>Loading head-to-head data...</div>;
  if (error) return <div className={styles.error}>Error: {error}</div>;
  if (!headToHeadData || filteredMatches.length === 0) return <div className={styles.noData}>No head-to-head data found</div>;

  return (
    <div className={styles.pageWrapper}>
      <Header />
      <div className={styles.contentContainer}>
        <div className={styles.headToHeadContainer}>
          <div className={styles.headToHeadHeader}>
            <h1 className={styles.headToHeadTitle}>Head-to-Head</h1>
            <div className={styles.playerVs}>
              <span className={styles.playerName}>{playerName} ({playerRating})</span>
              <span> vs </span>
              <span className={styles.opponentName}>{opponentName} ({opponentRating})</span>
            </div>
            
            <div className={styles.filters}>
              <div className={styles.filterGroup}>
                <label>Your Character:</label>
                <select 
                  value={yourCharacterFilter} 
                  onChange={(e) => setYourCharacterFilter(e.target.value)}
                >
                  <option value="All">All</option>
                  {yourCharacters.map(char => (
                    <option key={`your-${char}`} value={char}>{char}</option>
                  ))}
                </select>
              </div>
              
              <div className={styles.filterGroup}>
                <label>Opponent Character:</label>
                <select 
                  value={opponentCharacterFilter} 
                  onChange={(e) => setOpponentCharacterFilter(e.target.value)}
                >
                  <option value="All">All</option>
                  {opponentCharacters.map(char => (
                    <option key={`opp-${char}`} value={char}>{char}</option>
                  ))}
                </select>
              </div>
              
              <div className={styles.filterGroup}>
                <label>Result:</label>
                <select 
                  value={resultFilter} 
                  onChange={(e) => setResultFilter(e.target.value)}
                >
                  <option value="All">All</option>
                  <option value="Win">Win</option>
                  <option value="Loss">Loss</option>
                </select>
              </div>
            </div>
            
            <div className={styles.stats}>
              <span>{winCount}–{lossCount}</span>
              <span className={styles.winRate}>{winRate}%</span>
            </div>
          </div>

          <div className={styles.matchesList}>
            <div className={styles.matchHeader}>
              <span>Date</span>
              <span>Player</span>
              <span>Score</span>
              <span>Opponent</span>
              <span>Rating Change</span>
            </div>
            
            {filteredMatches.map(match => (
              <div 
                key={match.battleId} 
                className={`${styles.matchItem} ${match.winner ? styles.win : styles.loss}`}
              >
                <div className={styles.matchDate}>
                  {new Date(match.date).toLocaleString('en-GB', {
                    day: 'numeric',
                    month: 'short',
                    year: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}
                </div>
                <div className={styles.playerInfo}>
                  <span className={styles.playerName}>{playerName}</span>
                  <span className={styles.characterName}>{match.challenger.characterName}</span>
                  <span className={styles.rating}>{match.challenger.ratingBefore}</span>
                </div>
                <div className={styles.score}>
                  {match.challenger.rounds}-{match.opponent.rounds}
                </div>
                <div className={styles.opponentInfo}>
                  <span className={styles.playerName}>{opponentName}</span>
                  <span className={styles.characterName}>{match.opponent.characterName}</span>
                  <span className={styles.rating}>{match.opponent.ratingBefore}</span>
                </div>
                <div className={styles.ratingChange} data-positive={match.winner}>
                  {match.winner ? '+' : ''}{match.challenger.ratingChange}
                </div>
              </div>
            ))}
          </div>
          
          <div className={styles.footer}>
            Found {filteredMatches.length} games.
          </div>
        </div>
      </div>
    </div>
  );
};

export default HeadToHead;