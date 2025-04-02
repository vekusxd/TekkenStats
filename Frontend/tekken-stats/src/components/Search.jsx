import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePlayerSearch } from '../hooks/usePlayerSearch';
import styles from './TekkenStatsApp.module.css';
import Header from './Header';

const Search = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [inputError, setInputError] = useState(null);
  const navigate = useNavigate();
  const { searchPlayers, loading } = usePlayerSearch();

  const handleInputChange = (e) => {
    const value = e.target.value;
    setSearchQuery(value);
    setInputError(null);
  };

  const handleSearch = async (e) => {
    e.preventDefault();
    try {
      const results = await searchPlayers(searchQuery);
      if (results.length > 0) {
        navigate(`/${results[0].tekkenId}`);
      }
    } catch (error) {
      setInputError(error.message);
    }
  };

  return (
    <div className={styles.appContainer}>
      <Header />
      
      <main className={styles.main}>
        <div className={styles.hero}>
          <h1 className={styles.title}>Find Your Fighter Stats</h1>
          <div className={styles.mainSearchContainer}>
            <form onSubmit={handleSearch} className={styles.searchForm}>
              <div className={styles.searchInputWrapper}>
                <input
                  type="text"
                  placeholder="Search player name..."
                  className={`${styles.searchInput} ${inputError ? styles.inputError : ''}`}
                  value={searchQuery}
                  onChange={handleInputChange}
                />
                {inputError && (
                  <div className={styles.searchError}>
                    {inputError}
                  </div>
                )}
              </div>
              <button 
                type="submit" 
                className={styles.searchIcon}
                disabled={loading || !searchQuery.trim()}
              >
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.3-4.3"></path>
                </svg>
              </button>
            </form>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Search;