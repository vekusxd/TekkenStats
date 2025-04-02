import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { BASE_URL } from '../config/baseUrl';
import styles from './HeadToHead.module.css';
import { useParams } from 'react-router-dom';
import Header from './Header';

const HeadToHead = () => {
  const { tekkenId, opponentTekkenId } = useParams();
  const [headToHeadData, setHeadToHeadData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [yourCharacterFilter, setYourCharacterFilter] = useState('All');
  const [opponentCharacterFilter, setOpponentCharacterFilter] = useState('All');
  const [resultFilter, setResultFilter] = useState('All');

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await axios.get(`${BASE_URL}/api/head-to-head/${tekkenId}/${opponentTekkenId}`);
        setHeadToHeadData(response.data);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchData();
  }, [tekkenId, opponentTekkenId]);

  const filteredMatches = headToHeadData?.matches.filter(match => {
    const yourCharacterMatch = yourCharacterFilter === 'All' || 
      match.challenger.characterName === yourCharacterFilter;
    const opponentCharacterMatch = opponentCharacterFilter === 'All' || 
      match.opponent.characterName === opponentCharacterFilter;
    const resultMatch = resultFilter === 'All' || 
      (resultFilter === 'Win' && match.winner) || 
      (resultFilter === 'Loss' && !match.winner);
    return yourCharacterMatch && opponentCharacterMatch && resultMatch;
  });

  if (loading) return <div className={styles.loading}>Loading head-to-head data...</div>;
  if (error) return <div className={styles.error}>Error: {error}</div>;
  if (!headToHeadData) return <div className={styles.noData}>No head-to-head data found</div>;

  const playerName = headToHeadData.matches[0]?.challenger.name || 'Player';
  const opponentName = headToHeadData.matches[0]?.opponent.name || 'Opponent';

  const getLatestRating = (isChallenger) => {
    for (let i = headToHeadData.matches.length - 1; i >= 0; i--) {
      const rating = isChallenger 
        ? headToHeadData.matches[i].challenger.ratingBefore 
        : headToHeadData.matches[i].opponent.ratingBefore;
      if (rating !== 0) return rating;
    }
    return 0;
  };

  const playerRating = getLatestRating(true);
  const opponentRating = getLatestRating(false);

  const yourCharacters = [...new Set(
    headToHeadData.matches.map(m => m.challenger.characterName)
  )];
  const opponentCharacters = [...new Set(
    headToHeadData.matches.map(m => m.opponent.characterName)
  )];

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
            
            {/* ... остальной код без изменений ... */}
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
              <span>{headToHeadData.winCount}–{headToHeadData.lossCount}</span>
              <span className={styles.winRate}>{headToHeadData.challengerWinRate}%</span>
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