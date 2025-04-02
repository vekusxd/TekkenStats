import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import styles from './TekkenStatsProfile.module.css';
import Header from './Header';
import ProfileSummary from './profile/ProfileSummary';
import MatchHistory from './profile/MatchHistory';
import NameHistory from './profile/NameHistory';
import CharacterStatistics from './profile/CharacterStatistics';
import Rivals from './profile/Rivals';
import { BASE_URL } from '../config/baseUrl';
import Matchups from './profile/Matchups';

const TekkenStatsProfile = () => {
  const { tekkenId } = useParams();
  const [profile, setProfile] = useState(null);
  const [matches, setMatches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('matchHistory');
  const [filters, setFilters] = useState({
    playerCharacterId: null,
    opponentCharacterId: null,
    pageSize: 10,
    pageNumber: 1
  });
  const [opponentCharacters, setOpponentCharacters] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        const profileResponse = await axios.get(`${BASE_URL}/api/profile/${tekkenId}`);
        setProfile(profileResponse.data);
        
        const matchesResponse = await axios.get(`${BASE_URL}/api/matches/${tekkenId}`, {
          params: {
            PageSize: filters.pageSize,
            PageNumber: filters.pageNumber,
            PlayerCharacterId: filters.playerCharacterId,
            OpponentCharacterId: filters.opponentCharacterId
          }
        });
        setMatches(matchesResponse.data.matches);
        
        const matchupsResponse = await axios.get(`${BASE_URL}/api/matchups/${tekkenId}`);
        const matchups = matchupsResponse.data;
        const opponents = matchups.map(matchup => ({
          characterId: matchup.opponentCharacterId,
          characterName: matchup.characterName
        }));
        setOpponentCharacters(opponents.sort((a, b) => 
          a.characterName.localeCompare(b.characterName)
        ));
        
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchData();
  }, [tekkenId, filters]);

  if (loading) return <div className={styles.loading}>Loading...</div>;
  if (error) return <div className={styles.error}>Error: {error}</div>;
  if (!profile) return <div className={styles.noData}>No profile data</div>;

  return (
    <div className={styles.app}>
      <Header />
      <main className={`${styles.container} ${styles.main}`}>
        <div className={styles.content}>
          <ProfileSummary profile={profile} />
          
          <div className={styles.columns}>
            <div className={styles.leftColumn}>
              <div className={styles.tabsContainer}>
                <div className={styles.tabsHeader}>
                  <button 
                    className={`${styles.tabButton} ${activeTab === 'matchHistory' ? styles.tabButtonActive : ''}`}
                    onClick={() => setActiveTab('matchHistory')}
                  >
                    Match History
                  </button>
                  <button 
                    className={`${styles.tabButton} ${activeTab === 'rivals' ? styles.tabButtonActive : ''}`}
                    onClick={() => setActiveTab('rivals')}
                  >
                    Rivals
                  </button>
                  <button 
                    className={`${styles.tabButton} ${activeTab === 'Matchups' ? styles.tabButtonActive : ''}`}
                    onClick={() => setActiveTab('Matchups')}
                  >
                    Character Matchups
                  </button>
                </div>

                {activeTab === 'matchHistory' && (
                  <MatchHistory 
                    matches={matches} 
                    filters={filters} 
                    setFilters={setFilters} 
                    profile={profile}
                    opponentCharacters={opponentCharacters}
                  />
                )}

                {activeTab === 'rivals' && (
                  <Rivals 
                    tekkenId={tekkenId}
                    filters={filters}
                    setFilters={setFilters}
                    profile={profile}
                    opponentCharacters={opponentCharacters}
                  />
                )}

                {activeTab === 'Matchups' && (
                  <Matchups 
                    tekkenId={tekkenId}
                    profile={profile}
                  />
                )}
              </div>
            </div>

            <div className={styles.rightColumn}>
              <NameHistory names={profile.names} />
              <CharacterStatistics characters={profile.characters} />
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default TekkenStatsProfile;