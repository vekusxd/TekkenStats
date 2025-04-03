import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useInlineSearch } from '../hooks/useInlineSearch';
import styles from '../styles/TekkenStatsApp.module.css';
import Header from './Header';

const Search = () => {
  const navigate = useNavigate();
  const {
    searchQuery,
    inputError,
    suggestions,
    showSuggestions,
    loading,
    searchPlayers,
    handleInputChange,
    setSearchQuery,
    setShowSuggestions,
    setInputError
  } = useInlineSearch();

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

  const handlePlayerClick = (player) => {
    setSearchQuery(player.name);
    navigate(`/${player.tekkenId}`);
    setShowSuggestions(false);
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
                  onChange={(e) => handleInputChange(e.target.value)}
                  onFocus={() => searchQuery.length > 0 && setShowSuggestions(true)}
                  onBlur={() => setTimeout(() => setShowSuggestions(false), 200)}
                />
                {inputError && (
                  <div className={styles.searchError}>
                    {inputError}
                  </div>
                )}
                {showSuggestions && suggestions.length > 0 && (
                  <div className={styles.suggestionsDropdown}>
                    {suggestions.map(player => (
                      <div 
                        key={player.tekkenId}
                        className={styles.suggestionItem}
                        onClick={() => handlePlayerClick(player)}
                      >
                        {player.name}
                      </div>
                    ))}
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