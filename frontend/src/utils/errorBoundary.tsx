import React, { Component, ErrorInfo, ReactNode } from 'react';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
    error: null,
  };

  public static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Uncaught error:', error, errorInfo);
    console.error('Error stack:', error.stack);
    console.error('Component stack:', errorInfo.componentStack);
  }

  public render() {
    if (this.state.hasError) {
      return (
        <div style={{ padding: '20px', textAlign: 'center', fontFamily: 'Arial, sans-serif' }}>
          <h1 style={{ color: '#d32f2f' }}>Что-то пошло не так</h1>
          <p style={{ color: '#666', margin: '20px 0' }}>{this.state.error?.message}</p>
          {this.state.error?.stack && (
            <details style={{ margin: '20px 0', textAlign: 'left', maxWidth: '800px', marginLeft: 'auto', marginRight: 'auto' }}>
              <summary style={{ cursor: 'pointer', color: '#1976d2' }}>Детали ошибки</summary>
              <pre style={{ background: '#f5f5f5', padding: '10px', overflow: 'auto', fontSize: '12px' }}>
                {this.state.error.stack}
              </pre>
            </details>
          )}
          <button 
            onClick={() => window.location.reload()}
            style={{
              padding: '10px 20px',
              fontSize: '16px',
              backgroundColor: '#1976d2',
              color: 'white',
              border: 'none',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
          >
            Перезагрузить страницу
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;
