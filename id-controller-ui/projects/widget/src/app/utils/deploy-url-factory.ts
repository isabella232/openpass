export const deployUrl = () => {
  const openpassScriptsUrl = (document.querySelector('script[openpass]') as HTMLScriptElement).src;

  return openpassScriptsUrl?.slice(0, openpassScriptsUrl?.lastIndexOf('/'));
};
