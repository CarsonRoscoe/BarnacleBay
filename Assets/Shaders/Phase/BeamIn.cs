using UnityEngine;
using System.Collections;

namespace Phase {
    public class BeamIn : MonoBehaviour {
        private Renderer m_renderer;
        public float FazeInAmount = 100000;
        public float Speed = 1000;
        public float Distance = .1f;
        public float SecondsToSpawn = 1.5f;
        public Color BeamColor = new Color( 0, 1, 1 );
        private float m_amount;
        private float m_speed;
        private float m_distance;
        private Vector3 m_maxSize;
        private Vector3 m_size;
        private Shader m_oldShader;
        private Color m_oldColor;
        private Color m_currentColor;

        // Use this for initialization
        void Start() {
            m_renderer = GetComponent<Renderer>();
            m_oldShader = m_renderer.material.shader;
            m_oldColor = m_renderer.material.GetColor( "_Color" );
            m_renderer.material.shader = Shader.Find( "Custom/Phase" );
            m_renderer.material.SetFloat( "_Amount", FazeInAmount );
            m_amount = FazeInAmount;
            m_speed = Speed;
            m_distance = Distance;
            m_size = Vector3.zero;
            m_maxSize = transform.localScale;
            m_size.y = m_maxSize.y;
            transform.localScale = m_size;
        }

        // Update is called once per frame
        void Update() {
            if ( m_amount > 0 ) {
                m_amount -= FazeInAmount * (Time.deltaTime / SecondsToSpawn);
                m_speed = (m_amount / FazeInAmount) * Speed;
                m_distance = (m_amount / FazeInAmount) * Distance;
                m_size += m_maxSize * (Time.deltaTime / SecondsToSpawn);
                m_size.y = m_maxSize.y;
                var red = 1 - ((m_amount / FazeInAmount) *  m_oldColor.r) + (m_amount / FazeInAmount) * BeamColor.r;
                var green = 1 - ((m_amount / FazeInAmount) * m_oldColor.g) + (m_amount / FazeInAmount) * BeamColor.g;
                var blue = 1 - ((m_amount / FazeInAmount) * m_oldColor.b) + (m_amount / FazeInAmount) * BeamColor.b;
                m_currentColor = new Color( red, green, blue );
                transform.localScale = m_size;
                m_renderer.material.SetFloat( "_Amount", m_amount );
                m_renderer.material.SetFloat( "_Speed", m_speed );
                m_renderer.material.SetFloat( "_XDistance", m_distance / 2 );
                m_renderer.material.SetFloat( "_YDistance", m_distance * 100 );
                m_renderer.material.SetFloat( "_ZDistance", m_distance / 2 );
                m_renderer.material.SetColor( "_Color", m_currentColor );
                if ( m_amount < 0 ) {
                    m_amount = 0;
                }
            }
            else if ( m_amount == 0 ) {
                m_renderer.material.shader = m_oldShader;
                m_renderer.material.color = m_oldColor;
                Destroy( this );
            }
        }
    }
}
