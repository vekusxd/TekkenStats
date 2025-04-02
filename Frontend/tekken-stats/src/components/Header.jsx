import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { usePlayerSearch } from '../hooks/usePlayerSearch';
import styles from './TekkenStatsApp.module.css';

const Header = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [inputError, setInputError] = useState(null);
  const navigate = useNavigate();
  const { searchPlayers } = usePlayerSearch();

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
    <header className={styles.header}>
      <div className={styles.headerContainer}>
        <div className={styles.headerContent}>
          <a className={styles.logo} href="/">Tekken Stats</a>
          <div className={styles.headerSearchContainer}>
            <form onSubmit={handleSearch} className={styles.headerSearchForm}>
              <div className={styles.headerInputWrapper}>
                <input 
                  type="text" 
                  placeholder="Search player..." 
                  className={`${styles.headerSearchInput} ${inputError ? styles.inputError : ''}`}
                  value={searchQuery}
                  onChange={handleInputChange}
                />
                <button 
                  type="submit" 
                  className={styles.headerSearchIcon}
                  disabled={!searchQuery.trim()}
                >
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <circle cx="11" cy="11" r="8"></circle>
                    <path d="m21 21-4.3-4.3"></path>
                  </svg>
                </button>
              </div>
              {inputError && (
                <div className={styles.headerError}>
                  {inputError}
                </div>
              )}
            </form>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;